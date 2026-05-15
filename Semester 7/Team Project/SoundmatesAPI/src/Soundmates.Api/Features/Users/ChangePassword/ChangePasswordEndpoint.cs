using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.ChangePassword;

internal static class ChangePasswordEndpoint
{
    public static IEndpointRouteBuilder MapChangePassword(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/change-password", HandleAsync)
            .WithName("ChangePassword")
            .WithSummary("Change the authenticated user's password")
            .WithDescription("Verifies the old password and sets a new one.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Users")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<ChangePasswordRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] ChangePasswordRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: false, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        if (!authService.VerifyPasswordHash(request.OldPassword, user.PasswordHash))
            return TypedResults.Unauthorized();

        var trackedUser = await db.Users.FindAsync([user.Id], cancellationToken)
            ?? throw new InvalidOperationException($"User with id: {user.Id} was not found.");

        trackedUser.PasswordHash = authService.GetPasswordHash(request.NewPassword);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
