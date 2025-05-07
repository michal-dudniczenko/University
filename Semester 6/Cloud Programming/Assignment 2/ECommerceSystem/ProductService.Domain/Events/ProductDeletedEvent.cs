namespace ProductService.Domain.Events;

public class ProductDeletedEvent(Guid productId) : ProductEvent(productId)
{
}