using System.Security.Cryptography.X509Certificates;
using Common.Domain.Commands;

namespace NotificationService.Domain.Commands;

public class SendOrderNotificationCommand : Command
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public string Email { get; set; }

    public SendOrderNotificationCommand(Guid userId, Guid orderId, string email)
    {
        UserId = userId;
        OrderId = orderId;
        Email = email;
    }
}