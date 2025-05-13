using CartService.Domain.Commands;
using Common.Domain.Bus;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastracture.EventHandlers;

public class ProductDeletedEventHandler : IEventHandler<ProductDeletedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(IMediator mediator, ILogger<ProductDeletedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ProductDeletedEvent @event)
    {
        _logger.LogWarning("ProductDeletedEventHandler");

        await _mediator.Send(new ClearCartsContainingProductCommand(@event.ProductId));
    }
}