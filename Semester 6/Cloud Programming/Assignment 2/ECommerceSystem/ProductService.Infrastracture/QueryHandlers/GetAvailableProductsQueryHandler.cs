using MediatR;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.QueryHandlers;

public class GetAvailableProductsQueryHandler : IRequestHandler<GetAvailableProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAvailableProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> Handle(GetAvailableProductsQuery request, CancellationToken cancellationToken)
    {
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
