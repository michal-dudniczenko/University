using Common.Domain.Commands;

namespace CartService.Domain.Commands;

public class ClearCartsContainingProductCommand : Command
{
    public Guid ProductId { get; private set; }
    public ClearCartsContainingProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}