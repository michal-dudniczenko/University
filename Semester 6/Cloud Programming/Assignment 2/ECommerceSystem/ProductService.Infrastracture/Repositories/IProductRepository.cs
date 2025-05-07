using ProductService.Domain.Entities;

namespace ProductService.Infrastracture.Repositories;

public interface IProductRepository
{
    Task<Product?> GetProductById(Guid id);
    Task<List<Product>> GetAllProducts();
    Task<List<Product>> GetAvailableProducts();
    Task AddProduct(Product product);
    Task UpdateProduct(Product product);
    Task DeleteProduct(Guid id);
}
