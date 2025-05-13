namespace OrderService.Domain.DTO;

public class MakeOrderDto
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
}