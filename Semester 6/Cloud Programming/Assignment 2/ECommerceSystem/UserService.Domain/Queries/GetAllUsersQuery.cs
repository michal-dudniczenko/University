using MediatR;
using UserService.Domain.DTO;

namespace UserService.Domain.Queries;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}