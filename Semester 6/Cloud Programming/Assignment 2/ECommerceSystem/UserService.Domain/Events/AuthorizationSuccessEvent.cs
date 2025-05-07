using Common.Domain.Events;

namespace UserService.Domain.Events;

public class AuthorizationSuccessEvent(string token) : Event
{
    public string Token { get; } = token;
}