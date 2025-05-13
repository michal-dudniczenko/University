namespace Common.Domain.DTO;

public class CartEntryDto
{
    public required Guid ProductId { get; set; }
    public required Guid UserId { get; set; }
    public required int Quantity { get; set; }
}