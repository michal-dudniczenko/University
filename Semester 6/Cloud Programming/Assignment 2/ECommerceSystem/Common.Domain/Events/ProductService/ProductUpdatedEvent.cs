namespace Common.Domain.Events.ProductService;

public class ProductUpdatedEvent(Guid productId) : ProductEvent(productId)
{
}