namespace NotificationService.Domain.Events;

public class RegisterNotificationSentEvent(Guid userId, string Email) : NotificationEvent(userId, Email)
{
}