namespace Common.Domain.Events.UserService;

public class UserLoginFailureEvent : UserEvent
{
    public UserLoginFailureEvent(string userEmail) : base(userEmail)
    {
    }
}
