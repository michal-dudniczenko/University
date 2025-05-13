using Common.Domain.Bus;
using Common.Domain.Events.UserService;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Commands;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.CommandHandlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(IUserRepository userRepository, IEventBus eventBus, ILogger<LoginUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("LoginUserCommand");

        var user = await _userRepository.GetUserLogin(request.Email, request.Password);
        if (user == null)
        {
            await _eventBus.Publish(new UserLoginFailureEvent(request.Email));
            return false;
        }
        else
        {
            await _eventBus.Publish(new UserLoginSuccessEvent(user.Id, request.Email));
            return true;
        }
    }
}