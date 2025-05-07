using MediatR;
using ProductService.Domain.DTO;

namespace ProductService.Domain.Queries;

public class GetAvailableProductsQuery : IRequest<List<ProductDto>>
{
}