using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastracture.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<List<Product>> GetAvailableProducts()
    {
        return await _context.Products.Where(product => product.Quantity > 0).ToListAsync();
    }

    public async Task AddProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProduct(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);

        if (existingProduct is null)
            return;

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Quantity = product.Quantity;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteProduct(Guid id)
    {
        var product = await GetProductById(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}