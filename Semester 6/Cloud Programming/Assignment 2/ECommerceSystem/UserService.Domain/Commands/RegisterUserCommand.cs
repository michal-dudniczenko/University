namespace UserService.Domain.Commands;

public class RegisterUserCommand : UserCommand
{
    public RegisterUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
