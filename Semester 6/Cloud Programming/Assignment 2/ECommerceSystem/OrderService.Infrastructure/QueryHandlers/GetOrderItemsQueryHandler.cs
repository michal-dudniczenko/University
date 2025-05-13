using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.QueryHandlers;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, List<OrderItemDto>>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ILogger<GetOrderItemsQueryHandler> _logger;

    public GetOrderItemsQueryHandler(IOrderItemRepository orderItemRepository, ILogger<GetOrderItemsQueryHandler> logger)
    {
        _orderItemRepository = orderItemRepository;
        _logger = logger;
    }

    public async Task<List<OrderItemDto>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetOrderItemsQuery");

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