using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.Unmatch;

internal static class UnmatchEndpoint
{
    public static IEndpointRouteBuilder MapUnmatch(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/matching/unmatch/{userId}", HandleAsync)
            .WithName("Unmatch")
            .WithSummary("Unmatch with a user")
            .WithDescription("Removes the match between the authenticated user and the specified user.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Matching")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string userId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(userId, fieldName: "userId");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var userGuid = Guid.Parse(userId);

        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        if (userGuid == user.Id)
            return TypedResults.Problem(detail: "You cannot unmatch yourself.", statusCode: 400);

        var matchExists = await db.Matches.AnyAsync(
            m => (m.User1Id == user.Id && m.User2Id == userGuid) || (m.User1Id == userGuid && m.User2Id == user.Id),
            cancellationToken);

        if (!matchExists)
            return TypedResults.Problem(detail: $"Match with user : {userId} does not exist.", statusCode: 404);

        await db.Matches
            .Where(m => (m.User1Id == user.Id && m.User2Id == userGuid) || (m.User1Id == userGuid && m.User2Id == user.Id))
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
