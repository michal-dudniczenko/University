namespace Common.Domain.Events.ProductService;

public abstract class ProductEvent(Guid productId) : Event
{
    public Guid ProductId { get; } = productId;
}