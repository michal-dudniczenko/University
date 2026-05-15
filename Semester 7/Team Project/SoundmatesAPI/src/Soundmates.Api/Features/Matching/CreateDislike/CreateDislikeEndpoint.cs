using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.CreateDislike;

internal static class CreateDislikeEndpoint
{
    public static IEndpointRouteBuilder MapCreateDislike(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/dislike", HandleAsync)
            .WithName("CreateDislike")
            .WithSummary("Dislike a user")
            .WithDescription("Records a dislike reaction for the specified user.")
            .WithTags("Matching")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<CreateDislikeRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] CreateDislikeRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var receiverId = Guid.Parse(request.ReceiverId);

        if (receiverId == user.Id)
            return TypedResults.Problem(detail: "You cannot dislike your own profile.", statusCode: 400);

        var receiverExists = await db.Users.AnyAsync(
            u => u.Id == receiverId && u.IsActive && u.IsEmailConfirmed && !u.IsFirstLogin,
            cancellationToken);

        if (!receiverExists)
            return TypedResults.Problem(detail: $"No user with ID: {receiverId}", statusCode: 404);

        var reactionExists =
            await db.Likes.AnyAsync(l => l.GiverId == user.Id && l.ReceiverId == receiverId, cancellationToken) ||
            await db.Dislikes.AnyAsync(d => d.GiverId == user.Id && d.ReceiverId == receiverId, cancellationToken);

        if (reactionExists)
            return TypedResults.Problem(detail: $"Cannot give another reaction to the same user. From: {user.Id} To: {receiverId}", statusCode: 400);

        db.Dislikes.Add(new Dislike { GiverId = user.Id, ReceiverId = receiverId });
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
