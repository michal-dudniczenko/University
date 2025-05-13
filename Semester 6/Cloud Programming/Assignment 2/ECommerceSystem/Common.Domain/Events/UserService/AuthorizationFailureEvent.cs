namespace Common.Domain.Events.UserService;

public class AuthorizationFailureEvent(string token) : Event
{
    public string Token { get; } = token;
}