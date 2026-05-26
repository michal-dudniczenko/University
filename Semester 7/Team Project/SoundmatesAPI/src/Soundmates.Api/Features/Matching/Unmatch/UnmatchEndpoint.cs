using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
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
            .WithTags("Matching")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string userId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(userId, fieldName: "userId");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var userGuid = Guid.Parse(userId);

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
