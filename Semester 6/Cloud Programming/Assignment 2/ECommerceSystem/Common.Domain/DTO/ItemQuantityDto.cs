namespace Common.Domain.DTO;

public class ItemQuantityDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public ItemQuantityDto(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}