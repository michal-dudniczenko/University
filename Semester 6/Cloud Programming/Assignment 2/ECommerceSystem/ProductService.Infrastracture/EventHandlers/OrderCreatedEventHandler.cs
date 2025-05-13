using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Commands;

namespace ProductService.Infrastracture.EventHandlers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(IMediator mediator, ILogger<OrderCreatedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent @event)
    {
        _logger.LogWarning("OrderCreatedEventHandler");

        await _mediator.Send(new UpdateProductsQuantityCommand(@event.OrderItems));
    }
}