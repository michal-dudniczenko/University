namespace ProductService.Domain.Events;

public class ProductUpdatedEvent(Guid productId) : ProductEvent(productId)
{
}