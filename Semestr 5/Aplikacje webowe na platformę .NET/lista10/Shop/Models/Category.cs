namespace Shop.Models;

public class Category {
    public int Id { get; set; }
    public string? Name { get; set; }

    public override bool Equals(object? obj) {
        return obj is Category category && category.Id == this.Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }
}