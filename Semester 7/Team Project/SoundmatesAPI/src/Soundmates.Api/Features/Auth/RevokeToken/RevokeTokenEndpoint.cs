using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;

namespace Soundmates.Api.Features.Auth.RevokeToken;

internal static class RevokeTokenEndpoint
{
    public static IEndpointRouteBuilder MapRevokeToken(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/token/revoke", HandleAsync)
            .WithName("RevokeToken")
            .WithSummary("Revoke a refresh token")
            .WithDescription("Invalidates a single refresh token, signing out the current device.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<RevokeTokenRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RevokeTokenRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        await authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        return TypedResults.NoContent();
    }
}
