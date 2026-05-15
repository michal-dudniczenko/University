using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Soundmates.Api.Common.Hubs;

[Authorize]
internal sealed class EventHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value
                     ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine($"[Error] Connection attempted without valid 'sub' or 'NameIdentifier' claim. User Identity: {Context.User?.Identity?.Name ?? "Unknown"}");
            Context.Abort();
            return;
        }

        Console.WriteLine($"connected userId: {userId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}
