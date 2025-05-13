using Common.Domain.DTO;

namespace Common.Domain.Events.OrderService;

public class OrderCreatedEvent : Event
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public List<ItemQuantityDto> OrderItems { get; set; }

    public OrderCreatedEvent(Guid userId, Guid orderId, List<ItemQuantityDto> orderItems)
    {
        UserId = userId;
        OrderId = orderId;
        OrderItems = orderItems;
    }
}