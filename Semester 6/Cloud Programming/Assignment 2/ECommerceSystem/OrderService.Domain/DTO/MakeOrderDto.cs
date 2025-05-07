namespace OrderService.Domain.DTO;

public class MakeOrderDto
{
    public required Guid UserId { get; set; }
    public required List<ItemQuantityDto> OrderItems { get; set; }
}