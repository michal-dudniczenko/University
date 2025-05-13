namespace Common.Domain.Events.UserService;

public class AuthorizationSuccessEvent(string token) : Event
{
    public string Token { get; } = token;
}