namespace CartService.Domain.Entities;

public class CartEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid ProductId { get; set; }
    public required Guid UserId { get; set; }
    public required int Quantity { get; set; }
}