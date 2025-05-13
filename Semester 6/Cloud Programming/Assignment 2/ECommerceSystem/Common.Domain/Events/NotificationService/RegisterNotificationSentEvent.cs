namespace Common.Domain.Events.NotificationService;

public class RegisterNotificationSentEvent(Guid userId, string Email) : NotificationEvent(userId, Email)
{
}