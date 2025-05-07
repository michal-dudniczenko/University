using CartService.Domain.Commands;
using CartService.Domain.Events;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using MediatR;

namespace CartService.Infrastructure.CommandHandlers;

public class ClearUserCartCommandHandler : IRequestHandler<ClearUserCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;

    public ClearUserCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(ClearUserCartCommand request, CancellationToken cancellationToken)
    {
        await _cartRepository.ClearUserCart(request.UserId);
        await _eventBus.Publish(new UserCartClearedEvent(request.UserId));
        return true;
    }
}