namespace UserService.Domain.Events;

public class UserRegisteredEvent(string userEmail) : UserEvent(userEmail)
{
}
