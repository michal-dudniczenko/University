using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using System.Buffers.Text;
using System.Text;

namespace Soundmates.Api.Features.Auth.ResetPassword;

internal static class ResetPasswordEndpoint
{
    public static IEndpointRouteBuilder MapResetPassword(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/reset-password", HandleAsync)
            .WithName("ResetPassword")
            .WithSummary("Reset password")
            .WithDescription("Resets user password using received token.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<ResetPasswordRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ResetPasswordRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IAuthService authService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
        {
            return TypedResults.Problem(
                title: "Invalid token",
                detail: "The password reset link is invalid or has expired.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var decodedToken = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(request.Token));

        var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            loggerFactory.CreateLogger(nameof(ResetPasswordEndpoint))
                .LogWarning("Password reset failed. Errors: {Errors}", errors);

            return TypedResults.Problem(
                title: "Invalid token",
                detail: "The password reset link is invalid or has expired.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await authService.RevokeAllRefreshTokensAsync(user.Id, cancellationToken);

        return TypedResults.NoContent();
    }
}
