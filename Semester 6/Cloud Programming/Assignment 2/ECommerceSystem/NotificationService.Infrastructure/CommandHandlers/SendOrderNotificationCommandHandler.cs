using Common.Domain.Bus;
using MediatR;
using NotificationService.Domain.Commands;
using NotificationService.Domain.Events;

namespace NotificationService.Infrastructure.CommandHandlers;

public class SendOrderNotificationCommandHandler : IRequestHandler<SendOrderNotificationCommand, bool>
{
    private readonly IEventBus _eventBus;

    public SendOrderNotificationCommandHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(SendOrderNotificationCommand request, CancellationToken cancellationToken)
    {
        // TO DO
        Console.WriteLine(request.Email);
        await _eventBus.Publish(new OrderNotificationSentEvent(request.UserId, request.Email, request.OrderId));
        return true;
    }
}