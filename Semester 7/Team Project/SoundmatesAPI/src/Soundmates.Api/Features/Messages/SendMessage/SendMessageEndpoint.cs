using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Messages.SendMessage;

internal static class SendMessageEndpoint
{
    public static IEndpointRouteBuilder MapSendMessage(this IEndpointRouteBuilder app)
    {
        app.MapPost("/messages", HandleAsync)
            .WithName("SendMessage")
            .WithSummary("Send a message")
            .WithDescription("Sends a message to a matched user and notifies them via SignalR.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Messages")
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AddEndpointFilter<ValidationFilter<SendMessageRequest>>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] SendMessageRequest request,
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
            return TypedResults.Problem(detail: "You cannot send message to yourself.", statusCode: 400);

        var receiverExists = await db.Users.AnyAsync(
            u => u.Id == receiverId && u.IsActive && u.EmailConfirmed,
            cancellationToken);

        if (!receiverExists)
            return TypedResults.Problem(detail: $"No user with ID: {receiverId}", statusCode: 404);

        var matchExists = await db.Matches.AnyAsync(
            m => (m.User1Id == user.Id && m.User2Id == receiverId)
              || (m.User1Id == receiverId && m.User2Id == user.Id),
            cancellationToken);

        if (!matchExists)
            return TypedResults.Problem(detail: "You can send message only to users you have matched with.", statusCode: 401);

        db.Messages.Add(new Message
        {
            Content = request.Content,
            SenderId = user.Id,
            ReceiverId = receiverId
        });

        await db.SaveChangesAsync(cancellationToken);

        await hubContext.Clients.Group(receiverId.ToString()).SendAsync("MessageReceived", new
        {
            senderId = user.Id,
            senderName = user.Name
        }, cancellationToken);

        return TypedResults.Ok();
    }
}
