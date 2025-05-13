using Common.Domain.Bus;
using Common.Domain.Events.NotificationService;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Commands;

namespace NotificationService.Infrastructure.CommandHandlers;

public class SendRegisterNotificationCommandHandler : IRequestHandler<SendRegisterNotificationCommand, bool>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<SendRegisterNotificationCommandHandler> _logger;

    public SendRegisterNotificationCommandHandler(IEventBus eventBus, ILogger<SendRegisterNotificationCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(SendRegisterNotificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("SendRegisterNotificationCommand");

        // TO DO
        Console.WriteLine($"@@@@@ Email sent to: {request.Email} @@@@@");
        await _eventBus.Publish(new RegisterNotificationSentEvent(request.UserId, request.Email));
        return true;
    }
}