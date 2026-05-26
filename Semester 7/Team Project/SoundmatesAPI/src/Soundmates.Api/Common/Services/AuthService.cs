using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Options;
using Soundmates.Api.Persistence;
using System.Buffers.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Soundmates.Api.Common.Services;

internal interface IAuthService
{
    Task<User?> GetAuthorizedUserAsync(ClaimsPrincipal claimsPrincipal, bool checkForFirstLogin = true);
    Task SendEmailConfirmationAsync(string email, string token, HttpContext httpContext, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string token, HttpContext httpContext, CancellationToken cancellationToken = default);
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<string> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    string GenerateRandomToken();
    byte[] HashToken(string token);
}

internal sealed class AuthService(
    UserManager<User> userManager,
    IEmailService emailService,
    ApplicationDbContext db,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    public async Task<User?> GetAuthorizedUserAsync(
        ClaimsPrincipal claimsPrincipal,
        bool checkForFirstLogin = true)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        if (user is null || !user.EmailConfirmed || !user.IsActive)
            return null;

        if (checkForFirstLogin && user.IsFirstLogin)
            return null;

        return user;
    }

    public async Task SendEmailConfirmationAsync(
        string email,
        string token,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        var confirmationLink = $"{SecurityConstants.ConfirmEmailEndpointClientPath}?token={token}";

        await emailService.SendRegistrationConfirmationLinkAsync(email, confirmationLink, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string token,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        var encodedToken = Base64Url.EncodeToString(Encoding.UTF8.GetBytes(token));
        var resetLink = $"{SecurityConstants.ResetPasswordEndpointClientPath}?token={encodedToken}";

        await emailService.SendPasswordResetLinkAsync(email, resetLink, cancellationToken);
    }

    public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ..roles.Select(r => new Claim(ClaimTypes.Role, r))
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = jwtOptions.Value.Issuer,
            Audience = jwtOptions.Value.Audience,
        };

        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateRandomToken();
        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenLifetimeInDays)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);

        await db.RefreshTokens
            .Where(rt => rt.TokenHash == tokenHash)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await db.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var result = await ValidateAndRotateRefreshTokenAsync(refreshToken, cancellationToken);

        if (result is null)
            return null;

        var (user, newRawToken) = result.Value;
        var accessToken = await GenerateAccessTokenAsync(user, cancellationToken);

        return (AccessToken: accessToken, RefreshToken: newRawToken);
    }

    private async Task<(User, string)?> ValidateAndRotateRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);

        var existing = await db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

        if (existing is null)
            return null;

        if (existing.ExpiresAt < DateTime.UtcNow || !existing.User.IsActive || !existing.User.EmailConfirmed)
        {
            db.Remove(existing);
            await db.SaveChangesAsync(cancellationToken);
            return null;
        }

        var newRawToken = GenerateRandomToken();
        var newTokenHash = HashToken(newRawToken);

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = existing.UserId,
            TokenHash = newTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenLifetimeInDays)
        });
        db.Remove(existing);

        await db.SaveChangesAsync(cancellationToken);

        return (existing.User, newRawToken);
    }

    public string GenerateRandomToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);

        return Base64Url.EncodeToString(bytes);
    }

    public byte[] HashToken(string token) => SHA256.HashData(Encoding.UTF8.GetBytes(token));
}
