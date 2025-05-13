using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.QueryHandlers;

public class GetAvailableProductsQueryHandler : IRequestHandler<GetAvailableProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAvailableProductsQueryHandler> _logger;

    public GetAvailableProductsQueryHandler(IProductRepository productRepository, ILogger<GetAvailableProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(GetAvailableProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetAvailableProductsQuery");

        var products = await _productRepository.GetAvailableProducts();
        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        }).ToList();
    }

}
