using Common.Domain.Commands;

namespace OrderService.Domain.Commands;

public class MakeOrderCommand : Command
{
    public Guid UserId { get; set; }
    public string Token { get; set; }

    public MakeOrderCommand(Guid userId, string token)
    {
        UserId = userId;
        Token = token;
    }
}