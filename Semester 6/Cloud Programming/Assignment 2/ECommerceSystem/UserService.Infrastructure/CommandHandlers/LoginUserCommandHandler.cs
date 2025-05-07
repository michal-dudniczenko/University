using Common.Domain.Bus;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Commands;
using UserService.Domain.Events;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.CommandHandlers;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public LoginUserCommandHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserLogin(request.Email, request.Password);
        if (user == null)
        {
            await _eventBus.Publish(new UserLoginFailureEvent(request.Email));
            return false;
        }
        else
        {
            await _eventBus.Publish(new UserLoginSuccessEvent(request.Email));
            return true;
        }
    }
}