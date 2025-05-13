namespace Common.Domain.Events.NotificationService;

public class NotificationEvent : Event
{
    public Guid UserId { get; set; }
    public string Email { get; set; }

    public NotificationEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}