namespace Common.Domain.Events.UserService;
public class UserLoginSuccessEvent : UserEvent
{
    public Guid UserId { get; }

    public UserLoginSuccessEvent(Guid userId, string userEmail) : base(userEmail)
    {
        UserId = userId;
    }
}
