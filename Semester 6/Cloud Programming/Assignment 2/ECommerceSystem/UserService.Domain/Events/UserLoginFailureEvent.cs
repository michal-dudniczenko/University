namespace UserService.Domain.Events;

public class UserLoginFailureEvent(string userEmail) : UserEvent(userEmail)
{
}
