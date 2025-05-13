using Common.Domain.DTO;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Queries;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.QueryHandlers;

public class GetUserEmailQueryHandler : IRequestHandler<GetUserEmailQuery, UserEmailDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserEmailQueryHandler> _logger;

    public GetUserEmailQueryHandler(IUserRepository userRepository, ILogger<GetUserEmailQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserEmailDto> Handle(GetUserEmailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("GetUserEmailQuery");

        var userEmail = await _userRepository.GetUserEmail(request.Id);

        return new UserEmailDto
        {
            Email = userEmail
        };
    }
}