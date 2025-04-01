using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models;

public class Article {

    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? ImageFileName { get; set; } = null;

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public override bool Equals(object? obj) {
        return obj is Article article && article.Id == this.Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }
}