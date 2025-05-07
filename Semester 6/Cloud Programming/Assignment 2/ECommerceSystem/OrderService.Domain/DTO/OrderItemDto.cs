namespace OrderService.Domain.DTO;

public class OrderItemDto
{
    public required Guid Id { get; set; }
    public required Guid OrderId { get; set; }
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
}