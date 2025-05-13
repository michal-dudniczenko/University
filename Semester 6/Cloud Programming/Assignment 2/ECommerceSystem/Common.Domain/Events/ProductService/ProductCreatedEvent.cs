namespace Common.Domain.Events.ProductService;

public class ProductCreatedEvent(Guid productId) : ProductEvent(productId)
{
}