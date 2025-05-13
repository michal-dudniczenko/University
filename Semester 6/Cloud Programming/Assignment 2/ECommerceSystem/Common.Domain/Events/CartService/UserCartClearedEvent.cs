namespace Common.Domain.Events.CartService;

public class UserCartClearedEvent : Event
{
    public Guid UserId { get; private set; }

    public UserCartClearedEvent(Guid userId)
    {
        UserId = userId;
    }
}