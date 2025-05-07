using MediatR;
using ProductService.Domain.DTO;

namespace ProductService.Domain.Queries;

public class GetAllProductsQuery : IRequest<List<ProductDto>>
{
}