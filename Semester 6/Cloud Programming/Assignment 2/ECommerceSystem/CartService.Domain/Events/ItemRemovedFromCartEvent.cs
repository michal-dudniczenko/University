using Common.Domain.Events;

namespace CartService.Domain.Events;

public class ItemRemovedFromCartEvent : Event
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }

    public ItemRemovedFromCartEvent(Guid productId, Guid userId)
    {
        ProductId = productId;
        UserId = userId;
    }
}