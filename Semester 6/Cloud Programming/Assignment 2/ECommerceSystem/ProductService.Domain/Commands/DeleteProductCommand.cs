using Common.Domain.Commands;

namespace ProductService.Domain.Commands;

public class DeleteProductCommand : Command
{
    public Guid Id { get; protected set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}