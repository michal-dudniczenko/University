using CartService.Domain.Commands;
using Common.Domain.Bus;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastracture.EventHandlers;

public class ProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(IMediator mediator, ILogger<ProductUpdatedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ProductUpdatedEvent @event)
    {
        _logger.LogWarning("ProductUpdatedEventHandler");

        await _mediator.Send(new ClearCartsContainingProductCommand(@event.ProductId));
    }
}