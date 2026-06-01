using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Messages.ViewConversation;

internal static class ViewConversationEndpoint
{
    public static IEndpointRouteBuilder MapViewConversation(this IEndpointRouteBuilder app)
    {
        app.MapPost("/messages/{otherUserId}/view", HandleAsync)
            .WithName("ViewConversation")
            .WithSummary("Mark conversation as seen")
            .WithDescription("Marks all messages from the other user as seen and notifies them via SignalR.")
            .WithTags("Messages")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string otherUserId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] IHubContext<EventHub> hubContext,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(otherUserId, fieldName: "otherUserId");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var otherUserGuid = Guid.Parse(otherUserId);

        if (otherUserGuid == user.Id)
            return TypedResults.Problem(detail: "You cannot read your own conversation.", statusCode: 400);

        var otherUserExists = await db.Users.AnyAsync(
            u => u.Id == otherUserGuid && u.IsActive && u.EmailConfirmed,
            cancellationToken);

        if (!otherUserExists)
            return TypedResults.Problem(detail: $"No user with ID: {otherUserId}", statusCode: 404);

        var matchExists = await db.Matches.AnyAsync(
            m => (m.User1Id == user.Id && m.User2Id == otherUserGuid)
              || (m.User1Id == otherUserGuid && m.User2Id == user.Id),
            cancellationToken);

        if (!matchExists)
            return TypedResults.Problem(detail: "You can't have conversation with user that you have not matched with.", statusCode: 401);

        await db.Messages
            .Where(m => m.ReceiverId == user.Id && m.SenderId == otherUserGuid && !m.IsSeen)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsSeen, true), cancellationToken);

        await hubContext.Clients.Group(otherUserGuid.ToString()).SendAsync("ConversationSeen", new
        {
            userId = user.Id,
            timestamp = DateTime.UtcNow
        }, cancellationToken);

        return TypedResults.Ok();
    }
}
