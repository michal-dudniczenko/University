using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Auth.LogOut;

internal static class LogOutEndpoint
{
    public static IEndpointRouteBuilder MapLogOut(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", HandleAsync)
            .WithName("LogOut")
            .WithSummary("Log out the authenticated user")
            .WithDescription("Invalidates the current session and removes the refresh token.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: false, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        if (user.IsLoggedOut)
            return TypedResults.Problem(detail: "User is already logged out.", statusCode: 400);

        var trackedUser = await db.Users.FindAsync([user.Id], cancellationToken)
            ?? throw new InvalidOperationException($"User with id: {user.Id} was not found.");

        trackedUser.IsLoggedOut = true;

        var existingToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id, cancellationToken);

        if (existingToken is not null)
            db.RefreshTokens.Remove(existingToken);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
