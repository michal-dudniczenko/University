using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Reports.BlockUser;

internal static class BlockUserEndpoint
{
    public static IEndpointRouteBuilder MapBlockUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/reports/{userId}/block", HandleAsync)
            .WithName("BlockUser")
            .WithSummary("Block a user")
            .WithDescription("Deactivates a user account and revokes all active sessions. Admin only.")
            .WithTags("Reports")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .RequireAuthorization(SecurityConstants.PolicyRequireAdmin);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        string userId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] UserManager<User> userManager,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return TypedResults.Problem(detail: $"No user with ID: {userId}", statusCode: StatusCodes.Status404NotFound);

        var user = await userManager.FindByIdAsync(userGuid.ToString());

        if (user is null)
            return TypedResults.Problem(detail: $"No user with ID: {userId}", statusCode: StatusCodes.Status404NotFound);

        if (!user.IsActive)
            return TypedResults.Problem(detail: "User is already blocked.", statusCode: StatusCodes.Status409Conflict);

        user.IsActive = false;
        user.DeactivatedAt = DateTime.UtcNow;

        db.Users.Update(user);

        await userManager.UpdateSecurityStampAsync(user);

        await authService.RevokeAllRefreshTokensAsync(userGuid, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
