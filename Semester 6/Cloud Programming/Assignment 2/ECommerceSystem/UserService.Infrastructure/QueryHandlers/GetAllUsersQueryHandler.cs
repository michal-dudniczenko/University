using MediatR;
using UserService.Domain.DTO;
using UserService.Domain.Queries;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.QueryHandlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsers();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
        }).ToList();
    }
}