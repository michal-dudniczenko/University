using Common.Domain.Events;

namespace CartService.Domain.Events;

public class UserCartClearedEvent : Event
{
    public Guid UserId { get; private set; }

    public UserCartClearedEvent(Guid userId)
    {
        UserId = userId;
    }
}