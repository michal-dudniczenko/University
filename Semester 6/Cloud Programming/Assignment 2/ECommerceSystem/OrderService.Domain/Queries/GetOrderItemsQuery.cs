using MediatR;
using OrderService.Domain.DTO;

namespace OrderService.Domain.Queries;

public class GetOrderItemsQuery : IRequest<List<OrderItemDto>>
{
    public Guid OrderId { get; set; }

    public GetOrderItemsQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}