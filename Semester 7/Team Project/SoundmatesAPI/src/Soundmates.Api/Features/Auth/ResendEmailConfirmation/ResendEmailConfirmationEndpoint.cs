using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Auth.ResendEmailConfirmation;

internal static class ResendEmailConfirmationEndpoint
{
    public static IEndpointRouteBuilder MapResendEmailConfirmation(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/resend-email", HandleAsync)
            .WithName("ResendEmail")
            .WithSummary("Resend email confirmation")
            .WithDescription("Resends email with link to confirm email address.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<ResendEmailConfirmationRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ResendEmailConfirmationRequest request,
        [FromServices] IAuthService authService,
        [FromServices] ApplicationDbContext db,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var existingPending = await db.PendingRegistrations
            .Where(pr => pr.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingPending is null)
            return TypedResults.NoContent();

        var rawToken = authService.GenerateRandomToken();
        existingPending.EmailTokenHash = authService.HashToken(rawToken);
        existingPending.ExpiresAt = DateTime.UtcNow.AddMinutes(SecurityConstants.ConfirmEmailExpireDurationMinutes);

        await db.SaveChangesAsync(cancellationToken);

        await authService.SendEmailConfirmationAsync(
            request.Email,
            rawToken,
            httpContext,
            cancellationToken);

        return TypedResults.NoContent();
    }
}
