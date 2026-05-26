using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;

namespace Soundmates.Api.Features.Auth.ForgotPassword;

internal static class ForgotPasswordEndpoint
{
    public static IEndpointRouteBuilder MapForgotPassword(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/forgot-password", HandleAsync)
            .WithName("ForgotPassword")
            .WithSummary("Send password reset link")
            .WithDescription("Sends a link to reset password to specified email address")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<ForgotPasswordRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IAuthService authService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is not null && user.EmailConfirmed)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            await authService.SendPasswordResetEmailAsync(
                user.Email,
                token,
                httpContext,
                cancellationToken);
        }

        return TypedResults.NoContent();
    }
}
