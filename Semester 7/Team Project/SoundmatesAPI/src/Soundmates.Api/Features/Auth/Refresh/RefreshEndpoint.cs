using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;

namespace Soundmates.Api.Features.Auth.Refresh;

internal static class RefreshEndpoint
{
    public static IEndpointRouteBuilder MapRefresh(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", HandleAsync)
            .WithName("Refresh")
            .WithSummary("Refresh access token")
            .WithDescription("Exchanges a valid refresh token for a new access token.")
            .WithTags("Auth")
            .Produces<RefreshResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<RefreshRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RefreshRequest request,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (result is null)
            return TypedResults.Unauthorized();

        var (accessToken, refreshToken) = result.Value;

        return TypedResults.Ok(new RefreshResponse(AccessToken: accessToken, RefreshToken: refreshToken));
    }
}
