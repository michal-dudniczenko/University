using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Auth.DeactivateAccount;

internal static class DeactivateAccountEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/deactivate", HandleAsync)
            .WithName("DeactivateAccount")
            .WithSummary("Deactivate the authenticated user's account")
            .WithDescription("Permanently deactivates the account after password verification.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Auth")
            .AddEndpointFilter<ValidationFilter<DeactivateAccountRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] DeactivateAccountRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] UserManager<User> userManager,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal, checkForFirstLogin: false);
        if (user is null)
            return TypedResults.Unauthorized();

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            return TypedResults.Unauthorized();

        user.IsActive = false;
        user.DeactivatedAt = DateTime.UtcNow;

        db.Users.Update(user);

        await userManager.UpdateSecurityStampAsync(user);

        await authService.RevokeAllRefreshTokensAsync(user.Id, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
