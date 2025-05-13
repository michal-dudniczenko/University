using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.QueryHandlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(IProductRepository productRepository, ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetAllProductsQuery");

        var products = await _productRepository.GetAllProducts();
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