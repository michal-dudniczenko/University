using Common.Domain.Commands;

namespace ProductService.Domain.Commands;

public class UpdateProductCommand : Command
{
    public Guid Id { get; protected set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    public UpdateProductCommand(Guid id, string name, string description, decimal price, int quantity)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
    }
}