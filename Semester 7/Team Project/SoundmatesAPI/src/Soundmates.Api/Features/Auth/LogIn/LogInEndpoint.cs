using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Auth.LogIn;

internal static class LogInEndpoint
{
    public static IEndpointRouteBuilder MapLogIn(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", HandleAsync)
            .WithName("LogIn")
            .WithSummary("Authenticate a user")
            .WithDescription("Validates credentials and returns access and refresh tokens.")
            .WithTags("Auth")
            .Produces<LogInResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<LogInRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] LogInRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null || !authService.VerifyPasswordHash(request.Password, user.PasswordHash))
            return TypedResults.Unauthorized();

        if (!user.IsActive)
            return TypedResults.Problem(detail: "Your account has been deactivated. Contact administrator.", statusCode: 400);

        var accessToken = authService.GenerateAccessToken(user.Id);
        var refreshToken = authService.GenerateRefreshToken(user.Id);
        var refreshTokenHash = authService.GetRefreshTokenHash(refreshToken);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);

        await LogInUserAsync(db, user.Id, refreshTokenHash, refreshTokenExpiresAt, cancellationToken);

        return TypedResults.Ok(new LogInResponse(accessToken, refreshToken));
    }

    private static async Task LogInUserAsync(
        ApplicationDbContext db,
        Guid userId,
        string refreshTokenHash,
        DateTime refreshTokenExpiresAt,
        CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([userId], cancellationToken)
            ?? throw new InvalidOperationException($"User with id: {userId} was not found.");

        user.IsLoggedOut = false;

        var existingToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId, cancellationToken);

        if (existingToken is null)
        {
            db.RefreshTokens.Add(new RefreshToken
            {
                RefreshTokenHash = refreshTokenHash,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                UserId = userId
            });
        }
        else
        {
            existingToken.RefreshTokenHash = refreshTokenHash;
            existingToken.RefreshTokenExpiresAt = refreshTokenExpiresAt;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeEmail(string email)
    {
        email = email.Trim().ToUpperInvariant();

        var parts = email.Split('@');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid email format.", nameof(email));

        var local = parts[0];
        var domain = parts[1];

        if (domain == "gmail.com" || domain == "googlemail.com")
        {
            local = local.Split('+')[0];
            local = local.Replace(".", string.Empty, StringComparison.Ordinal);
        }

        return $"{local}@{domain}";
    }
}
