namespace Common.Domain.Events.NotificationService;

public class OrderNotificationSentEvent : NotificationEvent
{
    public Guid OrderId { get; set; }

    public OrderNotificationSentEvent(Guid userId, string email, Guid orderId) : base(userId, email)
    {
        OrderId = orderId;
    }
}