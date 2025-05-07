using Common.Domain.Events;

namespace UserService.Domain.Events;

public abstract class UserEvent(string userEmail) : Event
{
    public string UserEmail { get; } = userEmail;
}
