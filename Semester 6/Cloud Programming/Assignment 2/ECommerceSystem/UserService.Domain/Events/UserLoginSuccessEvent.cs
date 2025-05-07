namespace UserService.Domain.Events;

public class UserLoginSuccessEvent(string userEmail) : UserEvent(userEmail)
{
}
