using CartService.Domain.Commands;
using CartService.Domain.Events;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using MediatR;

namespace CartService.Infrastructure.CommandHandlers;

public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;

    public RemoveItemFromCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        await _cartRepository.RemoveItemFromCart(request.UserId, request.ProductId);
        await _eventBus.Publish(new ItemRemovedFromCartEvent(request.ProductId, request.UserId));
        return true;
    }
}