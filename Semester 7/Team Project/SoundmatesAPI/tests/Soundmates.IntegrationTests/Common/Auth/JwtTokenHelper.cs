using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Soundmates.Api.Common.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Soundmates.IntegrationTests.Common.Auth;

/// <summary>
/// Mints JWTs directly (bypassing the API) so tests can craft tokens the normal login flow can't:
/// expired tokens, wrong signing key / issuer / audience, or tokens for users that don't exist.
/// Valid tokens use the real signing material read from the host's <c>JwtOptions</c>.
/// </summary>
internal static class JwtTokenHelper
{
    public static Task<string> MintTokenAsync(
        this CustomWebApplicationFactory factory,
        Guid userId,
        string email,
        string? name = null,
        IEnumerable<string>? roles = null,
        TimeSpan? lifetime = null,
        string? secretKey = null,
        string? issuer = null,
        string? audience = null) =>
        factory.ExecuteScopeAsync(sp =>
        {
            var jwt = sp.GetRequiredService<IOptions<JwtOptions>>().Value;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Name, name ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange((roles ?? []).Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? jwt.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(jwt.ExpirationInMinutes));

            var token = new JwtSecurityToken(
                issuer: issuer ?? jwt.Issuer,
                audience: audience ?? jwt.Audience,
                claims: claims,
                // notBefore must precede expires even for already-expired tokens (negative lifetime),
                // so anchor it an hour before expiry rather than at "now".
                notBefore: expires.AddHours(-1),
                expires: expires,
                signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        });

    public static Task<string> MintExpiredTokenAsync(
        this CustomWebApplicationFactory factory, Guid userId, string email) =>
        factory.MintTokenAsync(userId, email, lifetime: TimeSpan.FromMinutes(-5));

    public static Task<string> MintWrongKeyTokenAsync(
        this CustomWebApplicationFactory factory, Guid userId, string email) =>
        factory.MintTokenAsync(userId, email, secretKey: TestConstants.WrongJwtSecretKey);

    public static Task<string> MintWrongIssuerTokenAsync(
        this CustomWebApplicationFactory factory, Guid userId, string email) =>
        factory.MintTokenAsync(userId, email, issuer: TestConstants.WrongJwtIssuer);

    public static Task<string> MintWrongAudienceTokenAsync(
        this CustomWebApplicationFactory factory, Guid userId, string email) =>
        factory.MintTokenAsync(userId, email, audience: TestConstants.WrongJwtAudience);
}
