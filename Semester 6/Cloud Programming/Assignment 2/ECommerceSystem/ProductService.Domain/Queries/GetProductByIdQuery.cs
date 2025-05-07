using MediatR;
using ProductService.Domain.DTO;

namespace ProductService.Domain.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; private set; }
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}