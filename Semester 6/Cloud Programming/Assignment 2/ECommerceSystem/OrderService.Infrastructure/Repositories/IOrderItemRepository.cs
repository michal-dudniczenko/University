using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetOrderItemById(Guid id);
    Task<List<OrderItem>> GetOrderItemsByOrderId(Guid orderId);
    Task CreateOrderItem(OrderItem orderItem);
}