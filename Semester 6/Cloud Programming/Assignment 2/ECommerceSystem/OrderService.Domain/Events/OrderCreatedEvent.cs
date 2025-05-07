using Common.Domain.Events;

namespace OrderService.Domain.Events;

public class OrderCreatedEvent : Event
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }

    public OrderCreatedEvent(Guid userId, Guid orderId)
    {
        UserId = userId;
        OrderId = orderId;
    }
}