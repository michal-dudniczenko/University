namespace Common.Domain.Events.UserService;

public class UserRegisteredEvent : UserEvent
{
    public Guid UserId { get; }

    public UserRegisteredEvent(Guid userId, string userEmail) : base(userEmail)
    {
        UserId = userId;
    }
}
