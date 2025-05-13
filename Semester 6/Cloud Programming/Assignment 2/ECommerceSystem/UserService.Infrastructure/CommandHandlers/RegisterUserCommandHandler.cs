using Common.Domain.Bus;
using Common.Domain.Events.UserService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Domain.Commands;
using UserService.Domain.Entities;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.CommandHandlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(IUserRepository userRepository, IEventBus eventBus, ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("RegisterUserCommand");

        var existing = await _userRepository.GetUserByEmail(request.Email);

        // Check if the user already exists
        if (existing != null)
        {
            return false;
        }
        else
        {
            var hasher = new PasswordHasher<object>();

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = hasher.HashPassword(new object(), request.Password)
            };

            await _userRepository.AddUser(newUser);
            await _eventBus.Publish(new UserRegisteredEvent(newUser.Id, request.Email));
            return true;
        }
    }
}