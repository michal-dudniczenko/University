using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.QueryHandlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(IProductRepository productRepository, ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetProductByIdQuery");

        var product = await _productRepository.GetProductById(request.Id);
        if (product == null)
        {
            return null;
        }
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        };
    }

}
