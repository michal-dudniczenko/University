using CartService.Domain.Commands;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using Common.Domain.Events.CartService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.CommandHandlers;

public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RemoveItemFromCartCommandHandler> _logger;

    public RemoveItemFromCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus, ILogger<RemoveItemFromCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("RemoveItemFromCartCommand");

        await _cartRepository.RemoveItemFromCart(request.UserId, request.ProductId);
        await _eventBus.Publish(new ItemRemovedFromCartEvent(request.ProductId, request.UserId));
        return true;
    }
}