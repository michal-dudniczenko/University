using CartService.Domain.Commands;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using Common.Domain.Events.CartService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.CommandHandlers;

public class ClearCartsContainingProductCommandHandler : IRequestHandler<ClearCartsContainingProductCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ClearCartsContainingProductCommandHandler> _logger;

    public ClearCartsContainingProductCommandHandler(ICartRepository cartRepository, IEventBus eventBus, ILogger<ClearCartsContainingProductCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(ClearCartsContainingProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("ClearCartsContainingProductCommand");

        var usersIds = await _cartRepository.GetUsersHavingProductInCart(request.ProductId);
        foreach (var userId in usersIds)
        {
            await _cartRepository.ClearUserCart(userId);
            await _eventBus.Publish(new UserCartClearedEvent(userId));
        }
        return true;
    }
}