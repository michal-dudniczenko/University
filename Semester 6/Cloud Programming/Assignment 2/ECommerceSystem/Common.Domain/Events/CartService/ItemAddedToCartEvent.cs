namespace Common.Domain.Events.CartService;

public class ItemAddedToCartEvent : Event
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }

    public ItemAddedToCartEvent(Guid productId, Guid userId)
    {
        ProductId = productId;
        UserId = userId;
    }
}