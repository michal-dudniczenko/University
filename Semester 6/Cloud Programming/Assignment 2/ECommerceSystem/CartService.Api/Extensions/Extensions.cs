using CartService.Infrastracture.EventHandlers;
using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using Common.Domain.Events.ProductService;

namespace CartService.Api.Extensions;

public static class Extensions
{
    public static void ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler, Domain.CartService>();
        eventBus.Subscribe<ProductDeletedEvent, ProductDeletedEventHandler, Domain.CartService>();
        eventBus.Subscribe<ProductUpdatedEvent, ProductUpdatedEventHandler, Domain.CartService>();
    }
}
