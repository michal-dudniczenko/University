using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Description { get; set; }

    [Column(TypeName = "DECIMAL(18, 2)")]
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
}