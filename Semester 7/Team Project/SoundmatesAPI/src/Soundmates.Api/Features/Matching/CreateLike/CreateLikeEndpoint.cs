using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.CreateLike;

internal static class CreateLikeEndpoint
{
    public static IEndpointRouteBuilder MapCreateLike(this IEndpointRouteBuilder app)
    {
        app.MapPost("/matching/like", HandleAsync)
            .WithName("CreateLike")
            .WithSummary("Like a user")
            .WithDescription("Records a like reaction. Creates a match if the other user already liked back.")
            .WithTags("Matching")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<CreateLikeRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] CreateLikeRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        [FromServices] IHubContext<EventHub> hubContext,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var receiverId = Guid.Parse(request.ReceiverId);

        if (receiverId == user.Id)
            return TypedResults.Problem(detail: "You cannot like your own profile.", statusCode: 400);

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

        db.Likes.Add(new Like { GiverId = user.Id, ReceiverId = receiverId });
        await db.SaveChangesAsync(cancellationToken);

        var reciprocalLikeExists = await db.Likes.AnyAsync(
            l => l.GiverId == receiverId && l.ReceiverId == user.Id,
            cancellationToken);

        if (reciprocalLikeExists)
        {
            db.Matches.Add(new Match { User1Id = user.Id, User2Id = receiverId });
            await db.SaveChangesAsync(cancellationToken);

            await hubContext.Clients.Group(receiverId.ToString()).SendAsync("MatchReceived", new
            {
                newLikeUserId = user.Id,
                newLikeUserName = user.Name
            }, cancellationToken);

            await hubContext.Clients.Group(user.Id.ToString()).SendAsync("MatchCreated", new
            {
                existingLikeUserId = receiverId
            }, cancellationToken);
        }

        return TypedResults.Ok();
    }
}
