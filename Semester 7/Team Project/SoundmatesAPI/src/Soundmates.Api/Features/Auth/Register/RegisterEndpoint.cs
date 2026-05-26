using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Auth.Register;

internal static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", HandleAsync)
            .WithName("Register")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account in *pending* state with the provided email and password.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterRequest request,
        [FromServices] IPasswordHasher<User> passwordHasher,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        await db.PendingRegistrations
            .Where(pr => pr.ExpiresAt < DateTime.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);

        var dummyUser = new User
        {
            Email = request.Email,
            UserName = request.Email,
        };

        string passwordHash = passwordHasher.HashPassword(dummyUser, request.Password);

        var rawToken = authService.GenerateRandomToken();
        var pending = new PendingRegistration
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            EmailTokenHash = authService.HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(SecurityConstants.ConfirmEmailExpireDurationMinutes)
        };

        db.PendingRegistrations.Add(pending);
        await db.SaveChangesAsync(cancellationToken);

        await authService.SendEmailConfirmationAsync(
            request.Email,
            rawToken,
            httpContext,
            cancellationToken);

        return TypedResults.NoContent();
    }
}
