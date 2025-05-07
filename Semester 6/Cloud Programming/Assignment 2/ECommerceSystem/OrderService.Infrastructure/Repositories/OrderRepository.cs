using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastracture.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task CreateOrder(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetOrderById(Guid id)
    {
        return await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task<List<Order>> GetUserOrders(Guid userId)
    {
        return await _context.Orders.Where(order => order.UserId == userId).ToListAsync();
    }
}