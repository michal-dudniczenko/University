using Common.Domain.Commands;
using OrderService.Domain.DTO;

namespace OrderService.Domain.Commands;

public class MakeOrderCommand : Command
{
    public Guid UserId { get; set; }
    public List<ItemQuantityDto> OrderItems { get; set; }

    public MakeOrderCommand(Guid userId, List<ItemQuantityDto> orderItems)
    {
        UserId = userId;
        OrderItems = orderItems;
    }
}