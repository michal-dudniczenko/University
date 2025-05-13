using CartService.Domain.Commands;
using CartService.Infrastructure.Repositories;
using Common.Domain.Bus;
using Common.Domain.Events.CartService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.CommandHandlers;

public class ClearUserCartCommandHandler : IRequestHandler<ClearUserCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ClearUserCartCommandHandler> _logger;

    public ClearUserCartCommandHandler(ICartRepository cartRepository, IEventBus eventBus, ILogger<ClearUserCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(ClearUserCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("ClearUserCartCommand");

        await _cartRepository.ClearUserCart(request.UserId);
        await _eventBus.Publish(new UserCartClearedEvent(request.UserId));
        return true;
    }
}