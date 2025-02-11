namespace Shop.Models;
    
public class ArticlesListContext : IArticlesContext {

    private readonly List<Article> _articles = [];
    private int currentId = 1;

    public List<Article> GetAll() => _articles;

    public Article? GetById(int id) => _articles.Find(a => a.Id == id);

    public bool Add(Article article) {
        article.Id = currentId;
        currentId++;
        _articles.Add(article);
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
        var existing = GetById(id);
        if (existing != null) {
            _articles.Remove(existing);
        }
        return true;
    }
}

