using CartService.Domain.DTO;
using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddItemToCart(Guid userId, Guid productId)
    {
        var currentEntry = await _context.CartEntries
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        if (currentEntry == null)
        {
            await _context.CartEntries.AddAsync(new CartEntry
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1
            });
        } else
        {
            currentEntry.Quantity++;
            _context.CartEntries.Update(currentEntry);
        }
        await _context.SaveChangesAsync();
    }

    public async Task ClearUserCart(Guid userId)
    {
        await _context.CartEntries
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync();

        await _context.SaveChangesAsync();
    }

    public async Task<List<CartEntryDto>> GetUserCartItems(Guid userId)
    {
        return await _context.CartEntries
            .Where(x => x.UserId == userId)
            .Select(x => new CartEntryDto
            {
                ProductId = x.ProductId,
                UserId = x.UserId,
                Quantity = x.Quantity
            })
            .ToListAsync();
    }

    public async Task RemoveItemFromCart(Guid userId, Guid productId)
    {
        var currentEntry = _context.CartEntries
            .FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);

        if (currentEntry != null)
        {
            if (currentEntry.Quantity > 1)
            {
                currentEntry.Quantity--;
                _context.CartEntries.Update(currentEntry);
            }
            else
            {
                _context.CartEntries.Remove(currentEntry);
            }
        }
        await _context.SaveChangesAsync();
    }
}