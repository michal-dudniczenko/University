using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using System.Security.Claims;

namespace Soundmates.Api.Features.Auth.ChangePassword;

internal static class ChangePasswordEndpoint
{
    public static IEndpointRouteBuilder MapChangePassword(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/change-password", HandleAsync)
            .WithName("ChangePassword")
            .WithSummary("Change the authenticated user's password")
            .WithDescription("Verifies the old password and sets a new one.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Auth")
            .AddEndpointFilter<ValidationFilter<ChangePasswordRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ChangePasswordRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == new IdentityErrorDescriber().PasswordMismatch().Code))
                return TypedResults.Unauthorized();

            var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));
        }

        await authService.RevokeAllRefreshTokensAsync(user.Id, cancellationToken);

        return TypedResults.NoContent();
    }
}
