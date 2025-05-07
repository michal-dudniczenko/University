using CartService.Domain.DTO;
using MediatR;

namespace CartService.Domain.Queries;

public class GetUserCartItemsQuery : IRequest<List<CartEntryDto>>
{
    public Guid UserId { get; private set; }
    public GetUserCartItemsQuery(Guid userId)
    {
        UserId = userId;
    }
}