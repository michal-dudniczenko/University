using Common.Domain.Bus;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Commands;
using UserService.Domain.Events;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.CommandHandlers;

public class AuthorizeUserCommandHandler : IRequestHandler<AuthorizeUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public AuthorizeUserCommandHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(AuthorizeUserCommand request, CancellationToken cancellationToken)
    {
        var hasher = new PasswordHasher<object>();

        var secretKey = await _userRepository.GetSecretKey();

        var token = request.Token;

        var tokenParts = token.Split('.');

        /* debugging
        if (tokenParts.Length != 3)
        {
            Console.WriteLine("wrong length");
        }

        if (hasher.VerifyHashedPassword(new object(), tokenParts[2], $"{tokenParts[0]}{tokenParts[1]}{secretKey}") != PasswordVerificationResult.Success)
        {
            Console.WriteLine("wrong hash");
        }

        if (long.Parse(tokenParts[1]) <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            Console.WriteLine("expired");
        }

        if ((await _userRepository.GetUserById(Guid.Parse(tokenParts[0]))) == null)
        {
            Console.WriteLine("user not found");
        }
        */

        if (
            (tokenParts.Length != 3) ||
            (hasher.VerifyHashedPassword(new object(), tokenParts[2], $"{tokenParts[0]}{tokenParts[1]}{secretKey}") != PasswordVerificationResult.Success) ||
            (long.Parse(tokenParts[1]) <= DateTimeOffset.UtcNow.ToUnixTimeSeconds()) ||
            ((await _userRepository.GetUserById(Guid.Parse(tokenParts[0]))) == null)
        )
        {
            await _eventBus.Publish(new AuthorizationFailureEvent(token));
            return false;
        }
        await _eventBus.Publish(new AuthorizationSuccessEvent(token));
        return true;
    }
}