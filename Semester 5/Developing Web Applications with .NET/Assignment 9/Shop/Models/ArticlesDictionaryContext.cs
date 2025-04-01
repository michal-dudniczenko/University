namespace Shop.Models;

public class ArticlesDictionaryContext : IArticlesContext {

    private readonly Dictionary<int, Article> _articles = [];
    private int currentId = 1;

    public List<Article> GetAll() => _articles.Values.ToList();
    
    public Article? GetById(int id) => _articles.GetValueOrDefault(id);

    public bool Add(Article article) {
        article.Id = currentId;
        currentId++;
        _articles[article.Id] = article;
        return true;
    }

    public bool Update(int id, Article article) {
        var existing = GetById(id);
        if (existing != null) {
            existing.Name = article.Name;
            existing.Price = article.Price;
            existing.ExpiryDate = article.ExpiryDate;
            existing.ArticleCategory = article.ArticleCategory;
        }
        return true;
    }

    public bool Delete(int id) {
        _articles.Remove(id);
        return true;
    }
}