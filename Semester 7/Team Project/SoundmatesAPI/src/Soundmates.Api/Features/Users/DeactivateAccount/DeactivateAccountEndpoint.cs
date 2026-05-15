using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.DeactivateAccount;

internal static class DeactivateAccountEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateAccount(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/users", HandleAsync)
            .WithName("DeactivateAccount")
            .WithSummary("Deactivate the authenticated user's account")
            .WithDescription("Permanently deactivates the account after password verification.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Users")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<DeactivateAccountRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] DeactivateAccountRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: false, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        if (!authService.VerifyPasswordHash(request.Password, user.PasswordHash))
            return TypedResults.Unauthorized();

        if (!user.IsActive)
            return TypedResults.Problem(detail: "User account has already been deactivated.", statusCode: 400);

        var trackedUser = await db.Users.FindAsync([user.Id], cancellationToken)
            ?? throw new InvalidOperationException($"User with id: {user.Id} was not found.");

        trackedUser.IsActive = false;
        trackedUser.IsLoggedOut = true;

        var existingToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id, cancellationToken);

        if (existingToken is not null)
            db.RefreshTokens.Remove(existingToken);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
