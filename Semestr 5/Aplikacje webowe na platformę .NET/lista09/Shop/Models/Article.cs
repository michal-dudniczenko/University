namespace Shop.Models;

public enum Category {
    Electronics,
    Groceries,
    Clothing,
    Home
}

public class Article {
    public int Id { get; set; } = -1;
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public Category ArticleCategory { get; set; }

    public override bool Equals(object? obj) {
        if (obj != null) {
            return ((Article)obj).Id == this.Id;
        } else {
            return false;
        } 
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

