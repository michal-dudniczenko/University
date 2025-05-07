using Common.Domain.Commands;

namespace CartService.Domain.Commands;

public class RemoveItemFromCartCommand : Command
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }

    public RemoveItemFromCartCommand(Guid productId, Guid userId)
    {
        ProductId = productId;
        UserId = userId;
    }
}