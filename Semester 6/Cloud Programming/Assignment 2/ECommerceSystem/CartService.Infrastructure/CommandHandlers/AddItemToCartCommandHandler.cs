using CartService.Domain.Commands;
using CartService.Domain.Events;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using MediatR;

namespace CartService.Infrastructure.CommandHandlers;

public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;

    public AddItemToCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        await _cartRepository.AddItemToCart(request.UserId, request.ProductId);
        await _eventBus.Publish(new ItemAddedToCartEvent(request.ProductId, request.UserId));
        return true;
    }
}