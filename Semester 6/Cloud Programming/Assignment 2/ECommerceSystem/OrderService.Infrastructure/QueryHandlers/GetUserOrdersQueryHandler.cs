using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.QueryHandlers;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetUserOrdersQueryHandler> _logger;

    public GetUserOrdersQueryHandler(IOrderRepository orderRepository, ILogger<GetUserOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<List<OrderDto>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetUserOrdersQuery");

        var userOrders = await _orderRepository.GetUserOrders(request.UserId);
        return userOrders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate
        }).ToList();
    }
}