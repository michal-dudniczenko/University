using CartService.Domain.DTO;

namespace CartService.Infrastructure.Repositories;

public interface ICartRepository
{
    Task<List<CartEntryDto>> GetUserCartItems(Guid userId);
    Task AddItemToCart(Guid userId, Guid productId);
    Task RemoveItemFromCart(Guid userId, Guid productId);
    Task ClearUserCart(Guid userId);
}