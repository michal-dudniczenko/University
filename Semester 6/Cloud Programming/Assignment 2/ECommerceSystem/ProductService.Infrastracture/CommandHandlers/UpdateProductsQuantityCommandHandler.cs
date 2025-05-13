using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Commands;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class UpdateProductsQuantityCommandHandler : IRequestHandler<UpdateProductsQuantityCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductsQuantityCommandHandler> _logger;

    public UpdateProductsQuantityCommandHandler(IProductRepository productRepository, ILogger<UpdateProductsQuantityCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }
    public async Task<bool> Handle(UpdateProductsQuantityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("UpdateProductsQuantityCommand");

        foreach (var item in request.Products)
        {
            var product = await _productRepository.GetProductById(item.ProductId);
            if (product != null)
            {
                product.Quantity = product.Quantity - item.Quantity;
                await _productRepository.UpdateProduct(product);
            }
        }
        return true;
    }
}