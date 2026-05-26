using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Auth.ConfirmEmail;

internal static class ConfirmEmailEndpoint
{
    public static IEndpointRouteBuilder MapConfirmEmail(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/confirm-email", HandleAsync)
            .WithName("ConfirmEmail")
            .WithSummary("Confirm email")
            .WithDescription("Confirms email provided during registration and completes registration process.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<ConfirmEmailRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ConfirmEmailRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IAuthService authService,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var tokenHash = authService.HashToken(request.Token);

        var pending = await db.PendingRegistrations
            .AsNoTracking()
            .Where(pr => pr.EmailTokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        if (pending is null || pending.ExpiresAt < DateTime.UtcNow)
        {
            return TypedResults.Problem(
                title: "Invalid token",
                detail: "Provided token is invalid or expired. You can issue new one at /auth/register.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var user = new User
        {
            Email = pending.Email,
            UserName = pending.Email,
            PasswordHash = pending.PasswordHash,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException("User creation failed.");

        var defaultMatchPreference = new UserMatchPreference { UserId = user.Id };
        db.UserMatchPreferences.Add(defaultMatchPreference);

        await db.PendingRegistrations
            .Where(pr => pr.Email == user.Email)
            .ExecuteDeleteAsync(cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
