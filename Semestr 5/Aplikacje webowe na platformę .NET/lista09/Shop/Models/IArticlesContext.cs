namespace Shop.Models;

public interface IArticlesContext {
    List<Article> GetAll();
    Article? GetById(int id);
    bool Add(Article article);
    bool Update(int id, Article article);
    bool Delete(int id);
}
