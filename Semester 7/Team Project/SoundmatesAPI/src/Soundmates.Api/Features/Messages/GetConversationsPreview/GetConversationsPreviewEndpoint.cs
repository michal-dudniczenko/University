using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Features.Messages.GetConversation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Messages.GetConversationsPreview;

internal static class GetConversationsPreviewEndpoint
{
    public static IEndpointRouteBuilder MapGetConversationsPreview(this IEndpointRouteBuilder app)
    {
        app.MapGet("/messages/preview", HandleAsync)
            .WithName("GetConversationsPreview")
            .WithSummary("Get conversations preview")
            .WithDescription("Returns the latest message from each conversation the current user is part of.")
            .Produces<List<MessageResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Messages")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var userMessages = await db.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
            .ToListAsync(cancellationToken);

        var latestMessages = userMessages
            .GroupBy(m => new
            {
                User1Id = m.SenderId < m.ReceiverId ? m.SenderId : m.ReceiverId,
                User2Id = m.SenderId < m.ReceiverId ? m.ReceiverId : m.SenderId
            })
            .Select(g => g.OrderByDescending(m => m.Timestamp).First())
            .OrderByDescending(m => m.Timestamp)
            .Select(m => new MessageResponse(m.Content, m.Timestamp, m.SenderId, m.ReceiverId, m.IsSeen))
            .ToList();

        return TypedResults.Ok(latestMessages);
    }
}
