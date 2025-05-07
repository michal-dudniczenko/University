namespace ProductService.Domain.Events;

public class ProductCreatedEvent(Guid productId) : ProductEvent(productId)
{
}