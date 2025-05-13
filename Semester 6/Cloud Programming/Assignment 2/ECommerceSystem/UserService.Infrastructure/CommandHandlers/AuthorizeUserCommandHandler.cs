using Common.Domain.Bus;
using Common.Domain.Events.UserService;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Commands;
using UserService.Infrastructure.Helpers;

namespace UserService.Infrastructure.CommandHandlers;

public class AuthorizeUserCommandHandler : IRequestHandler<AuthorizeUserCommand, bool>
{
    private readonly ITokenService _tokenService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AuthorizeUserCommandHandler> _logger;

    public AuthorizeUserCommandHandler(ITokenService tokenService, IEventBus eventBus, ILogger<AuthorizeUserCommandHandler> logger)
    {
        _tokenService = tokenService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(AuthorizeUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("AuthorizeUserCommand");

        if ((await _tokenService.ValidateToken(request.Token)) == true)
        {
            await _eventBus.Publish(new AuthorizationSuccessEvent(request.Token));
            return true;
        } 
        else
        {
            await _eventBus.Publish(new AuthorizationFailureEvent(request.Token));
            return false;
        }
    }
}