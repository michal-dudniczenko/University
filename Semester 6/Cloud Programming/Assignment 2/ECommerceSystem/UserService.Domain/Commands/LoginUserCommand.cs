namespace UserService.Domain.Commands;

public class LoginUserCommand : UserCommand
{
    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
