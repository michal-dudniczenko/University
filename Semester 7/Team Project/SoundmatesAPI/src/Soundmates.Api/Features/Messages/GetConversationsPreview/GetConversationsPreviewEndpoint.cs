using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Services;
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
            .WithTags("Messages");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var latestMessages = await db.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
            .Where(m => !db.Messages.Any(newer =>
                ((newer.SenderId == m.SenderId && newer.ReceiverId == m.ReceiverId)
                    || (newer.SenderId == m.ReceiverId && newer.ReceiverId == m.SenderId))
                && (newer.CreatedAt > m.CreatedAt
                    || (newer.CreatedAt == m.CreatedAt && newer.Id > m.Id))))
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MessageResponse(m.Content, m.CreatedAt, m.SenderId, m.ReceiverId, m.IsSeen))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(latestMessages);
    }
}
