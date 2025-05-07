namespace UserService.Domain.Commands;

public class AuthorizeUserCommand : UserCommand
{
    public AuthorizeUserCommand(string token)
    {
        Token = token;
    }
}
