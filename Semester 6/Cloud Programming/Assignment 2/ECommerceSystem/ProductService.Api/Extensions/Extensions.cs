using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using ProductService.Infrastracture.EventHandlers;

namespace ProductService.Api.Extensions;

public static class Extensions
{
    public static void ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler, Domain.ProductService>();
    }
}
