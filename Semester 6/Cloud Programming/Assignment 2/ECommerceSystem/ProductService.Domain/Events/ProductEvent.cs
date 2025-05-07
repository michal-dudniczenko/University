using Common.Domain.Events;

namespace ProductService.Domain.Events;

public abstract class ProductEvent(Guid productId) : Event
{
    public Guid ProductId { get; } = productId;
}