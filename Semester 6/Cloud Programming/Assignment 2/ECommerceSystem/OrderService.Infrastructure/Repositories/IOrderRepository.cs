using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderById(Guid id);
    Task<List<Order>> GetUserOrders(Guid userId);
    Task CreateOrder(Order order);
}