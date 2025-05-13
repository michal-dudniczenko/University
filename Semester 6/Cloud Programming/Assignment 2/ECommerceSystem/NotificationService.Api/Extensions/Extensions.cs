using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using Common.Domain.Events.UserService;
using NotificationService.Infrastracture.EventHandlers;

namespace NotificationService.Api.Extensions;

public static class Extensions
{
    public static void ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler, Domain.NotificationService>();
        eventBus.Subscribe<UserRegisteredEvent, UserRegisteredEventHandler, Domain.NotificationService>();
    }
}
