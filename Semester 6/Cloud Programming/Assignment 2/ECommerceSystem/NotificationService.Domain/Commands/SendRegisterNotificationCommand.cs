using Common.Domain.Commands;

namespace NotificationService.Domain.Commands;

public class SendRegisterNotificationCommand : Command
{
    public Guid UserId { get; set; }
    public string Email { get; set; }

    public SendRegisterNotificationCommand(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}