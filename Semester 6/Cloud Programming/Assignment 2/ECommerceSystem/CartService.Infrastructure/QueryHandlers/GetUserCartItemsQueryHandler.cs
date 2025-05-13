using CartService.Domain.Queries;
using CartService.Infrastructure.Repositories;
using Common.Domain.DTO;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Infrastructure.QueryHandlers;

public class GetUserCartItemsQueryHandler : IRequestHandler<GetUserCartItemsQuery, List<CartEntryDto>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<GetUserCartItemsQueryHandler> _logger;

    public GetUserCartItemsQueryHandler(ICartRepository cartRepository, ILogger<GetUserCartItemsQueryHandler> logger)
    {
        _cartRepository = cartRepository;
        _logger = logger;
    }

    public async Task<List<CartEntryDto>> Handle(GetUserCartItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetUserCartItemsQuery");

        return await _cartRepository.GetUserCartItems(request.UserId);
    }
}