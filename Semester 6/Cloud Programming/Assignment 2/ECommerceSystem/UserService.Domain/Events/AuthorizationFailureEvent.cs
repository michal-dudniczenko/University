using Common.Domain.Events;

namespace UserService.Domain.Events;

public class AuthorizationFailureEvent(string token) : Event
{
    public string Token { get; } = token;
}