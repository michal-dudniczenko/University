using Common.Domain.Bus;
using Common.Domain.Events.UserService;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Commands;

namespace NotificationService.Infrastracture.EventHandlers;

public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IMediator mediator, ILogger<UserRegisteredEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent @event)
    {
        _logger.LogWarning("UserRegisteredEventHandler");

        await _mediator.Send(new SendRegisterNotificationCommand(@event.UserId, @event.UserEmail));
    }
}