using MediatR;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.QueryHandlers;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetUserOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderDto>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var userOrders = await _orderRepository.GetUserOrders(request.UserId);
        return userOrders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate
        }).ToList();
    }
}