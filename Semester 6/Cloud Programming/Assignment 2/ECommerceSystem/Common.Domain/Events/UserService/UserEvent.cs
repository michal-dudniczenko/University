namespace Common.Domain.Events.UserService;

public abstract class UserEvent : Event
{
    public string UserEmail { get; }

    public UserEvent(string userEmail)
    {
        UserEmail = userEmail;
    }
}
