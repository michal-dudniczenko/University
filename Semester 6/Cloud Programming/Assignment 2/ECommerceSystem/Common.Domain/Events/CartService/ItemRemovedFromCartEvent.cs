namespace Common.Domain.Events.CartService;

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