using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.DTO;
using UserService.Domain.Queries;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.QueryHandlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IUserRepository userRepository, ILogger<GetAllUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetAllUsersQuery");

        var users = await _userRepository.GetAllUsers();
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
        }).ToList();
    }
}