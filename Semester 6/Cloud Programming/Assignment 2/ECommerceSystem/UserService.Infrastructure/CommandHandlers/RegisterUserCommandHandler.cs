using Common.Domain.Bus;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.CommandHandlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public RegisterUserCommandHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
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
            await _eventBus.Publish(new UserRegisteredEvent(request.Email));
            return true;
        }
    }
}