using CartService.Domain.DTO;
using CartService.Domain.Queries;
using CartService.Infrastructure.Repositories;
using MediatR;

namespace CartService.Infrastructure.QueryHandlers;

public class GetUserCartItemsQueryHandler : IRequestHandler<GetUserCartItemsQuery, List<CartEntryDto>>
{
    private readonly ICartRepository _cartRepository;

    public GetUserCartItemsQueryHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<List<CartEntryDto>> Handle(GetUserCartItemsQuery request, CancellationToken cancellationToken)
    {
        return await _cartRepository.GetUserCartItems(request.UserId);
    }
}