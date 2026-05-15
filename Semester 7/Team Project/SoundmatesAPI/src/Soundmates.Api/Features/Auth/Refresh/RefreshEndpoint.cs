using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;

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
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<RefreshRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] RefreshRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var refreshTokenHash = authService.GetRefreshTokenHash(request.RefreshToken);

        var refreshToken = await db.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.RefreshTokenHash == refreshTokenHash && rt.RefreshTokenExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync(cancellationToken);

        if (refreshToken is null)
            return TypedResults.Unauthorized();

        var accessToken = authService.GenerateAccessToken(refreshToken.UserId);

        return TypedResults.Ok(new RefreshResponse(accessToken));
    }
}
