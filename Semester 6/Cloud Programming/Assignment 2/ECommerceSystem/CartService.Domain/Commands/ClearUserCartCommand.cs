using Common.Domain.Commands;

namespace CartService.Domain.Commands;

public class ClearUserCartCommand : Command
{
    public Guid UserId { get; private set; }

    public ClearUserCartCommand(Guid userId)
    {
        UserId = userId;
    }
}