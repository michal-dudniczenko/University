using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.MatchExists;

internal static class MatchExistsEndpoint
{
    public static IEndpointRouteBuilder MapMatchExists(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/match/exists/{userId}", HandleAsync)
            .WithName("MatchExists")
            .WithSummary("Check if a match exists")
            .WithDescription("Returns true if the authenticated user has a match with the specified user.")
            .Produces<bool>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
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

        if (user.Id == userGuid)
            return TypedResults.Problem(detail: "User can't have matched yourself.", statusCode: 400);

        var matchExists = await db.Matches.AnyAsync(
            m => (m.User1Id == user.Id && m.User2Id == userGuid) || (m.User1Id == userGuid && m.User2Id == user.Id),
            cancellationToken);

        return TypedResults.Ok(matchExists);
    }
}
