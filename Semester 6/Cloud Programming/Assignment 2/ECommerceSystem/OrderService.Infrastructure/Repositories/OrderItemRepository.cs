using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastracture.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _context;

    public OrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateOrderItem(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderItem?> GetOrderItemById(Guid id)
    {
        return await _context.OrderItems.FirstOrDefaultAsync(orderItem => orderItem.Id == id);
    }

    public async Task<List<OrderItem>> GetOrderItemsByOrderId(Guid orderId)
    {
        return await _context.OrderItems
            .Where(orderItem => orderItem.OrderId == orderId)
            .ToListAsync();
    }
}