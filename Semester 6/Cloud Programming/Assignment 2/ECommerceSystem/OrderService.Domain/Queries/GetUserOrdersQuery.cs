using MediatR;
using OrderService.Domain.DTO;

namespace OrderService.Domain.Queries;

public class GetUserOrdersQuery : IRequest<List<OrderDto>>
{
    public Guid UserId { get; set; }

    public GetUserOrdersQuery(Guid userId)
    {
        UserId = userId;
    }
}