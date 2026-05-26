using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using System.Security.Claims;

namespace Soundmates.Api.Features.Auth.RevokeAllTokens;

internal static class RevokeAllTokensEndpoint
{
    public static IEndpointRouteBuilder MapRevokeAllTokens(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/token/revoke-all", HandleAsync)
            .WithName("RevokeAllTokens")
            .WithSummary("Revoke all refresh tokens")
            .WithDescription("Invalidates all refresh tokens for the authenticated user, signing out all devices.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal claimsPrincipal,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(claimsPrincipal, checkForFirstLogin: false);

        if (user is null)
            return TypedResults.Unauthorized();

        await authService.RevokeAllRefreshTokensAsync(user.Id, cancellationToken);
        return TypedResults.NoContent();
    }
}
