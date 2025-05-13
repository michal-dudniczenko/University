using Common.Domain.Bus;
using Common.Domain.Events.NotificationService;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Commands;

namespace NotificationService.Infrastructure.CommandHandlers;

public class SendOrderNotificationCommandHandler : IRequestHandler<SendOrderNotificationCommand, bool>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<SendOrderNotificationCommandHandler> _logger;

    public SendOrderNotificationCommandHandler(IEventBus eventBus, ILogger<SendOrderNotificationCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(SendOrderNotificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("SendOrderNotificationCommand");

        // TO DO
        Console.WriteLine($"@@@@@ Email sent to: {request.Email} @@@@@");
        await _eventBus.Publish(new OrderNotificationSentEvent(request.UserId, request.Email, request.OrderId));
        return true;
    }
}