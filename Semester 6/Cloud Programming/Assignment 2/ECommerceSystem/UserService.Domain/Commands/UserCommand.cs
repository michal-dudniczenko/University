using Common.Domain.Commands;

namespace UserService.Domain.Commands;

public class UserCommand : Command
{
    public string? Email { get; protected set; }
    public string? Password { get; protected set; }
    public string? Token { get; protected set; }
}
