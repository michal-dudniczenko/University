namespace Common.Domain.Events.ProductService;

public class ProductDeletedEvent(Guid productId) : ProductEvent(productId)
{
}