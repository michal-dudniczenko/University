using Common.Domain.Bus;
using MediatR;
using NotificationService.Domain.Commands;
using NotificationService.Domain.Events;

namespace NotificationService.Infrastructure.CommandHandlers;

public class SendRegisterNotificationCommandHandler : IRequestHandler<SendRegisterNotificationCommand, bool>
{
    private readonly IEventBus _eventBus;

    public SendRegisterNotificationCommandHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(SendRegisterNotificationCommand request, CancellationToken cancellationToken)
    {
        // TO DO
        Console.WriteLine(request.Email);
        await _eventBus.Publish(new RegisterNotificationSentEvent(request.UserId, request.Email));
        return true;
    }
}