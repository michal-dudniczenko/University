using MediatR;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.QueryHandlers;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, List<OrderItemDto>>
{
    private readonly IOrderItemRepository _orderItemRepository;

    public GetOrderItemsQueryHandler(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public async Task<List<OrderItemDto>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var orderItems = await _orderItemRepository.GetOrderItemsByOrderId(request.OrderId);
        return orderItems.Select(orderItem => new OrderItemDto
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity
        }).ToList();
    }
}