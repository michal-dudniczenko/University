using Common.Domain.DTO;
using MediatR;

namespace UserService.Domain.Queries;

public class GetUserEmailQuery : IRequest<UserEmailDto>
{
    public Guid Id { get; set; }
    public GetUserEmailQuery(Guid id)
    {
        Id = id;
    }
}