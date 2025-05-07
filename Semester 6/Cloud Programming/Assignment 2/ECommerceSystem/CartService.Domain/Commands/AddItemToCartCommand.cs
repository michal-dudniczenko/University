using Common.Domain.Commands;

namespace CartService.Domain.Commands;

public class AddItemToCartCommand : Command
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }

    public AddItemToCartCommand(Guid productId, Guid userId)
    {
        ProductId = productId;
        UserId = userId;
    }
}