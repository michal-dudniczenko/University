using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Common.Services;
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
            .AddEndpointFilter<ValidationFilter<CreateLikeRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateLikeRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] IHubContext<EventHub> hubContext,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var receiverId = Guid.Parse(request.ReceiverId);

        if (receiverId == user.Id)
            return TypedResults.Problem(detail: "You cannot like your own profile.", statusCode: 400);

        var receiverExists = await db.Users.AnyAsync(
            u => u.Id == receiverId && u.IsActive && u.EmailConfirmed && !u.IsFirstLogin,
            cancellationToken);

        if (!receiverExists)
            return TypedResults.Problem(detail: $"No user with ID: {receiverId}", statusCode: 404);

        var reactionExists =
            await db.Likes.AnyAsync(l => l.GiverId == user.Id && l.ReceiverId == receiverId, cancellationToken) ||
            await db.Dislikes.AnyAsync(d => d.GiverId == user.Id && d.ReceiverId == receiverId, cancellationToken);

        if (reactionExists)
            return TypedResults.Problem(detail: $"Cannot give another reaction to the same user. From: {user.Id} To: {receiverId}", statusCode: 400);

        // The reciprocal like (receiver -> user) is independent of the like we are about to add,
        // so we can check it first and persist the new like (and the match, if any) in a single round trip.
        var reciprocalLikeExists = await db.Likes.AnyAsync(
            l => l.GiverId == receiverId && l.ReceiverId == user.Id,
            cancellationToken);

        db.Likes.Add(new Like { GiverId = user.Id, ReceiverId = receiverId });
        if (reciprocalLikeExists)
            db.Matches.Add(new Match { User1Id = user.Id, User2Id = receiverId });

        await db.SaveChangesAsync(cancellationToken);

        if (reciprocalLikeExists)
        {
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
