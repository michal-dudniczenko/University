using CartService.Domain.Commands;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using Common.Domain.Events.CartService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.CommandHandlers;

public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AddItemToCartCommandHandler> _logger;

    public AddItemToCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus, ILogger<AddItemToCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("AddItemToCartCommand");

        await _cartRepository.AddItemToCart(request.UserId, request.ProductId);
        await _eventBus.Publish(new ItemAddedToCartEvent(request.ProductId, request.UserId));
        return true;
    }
}