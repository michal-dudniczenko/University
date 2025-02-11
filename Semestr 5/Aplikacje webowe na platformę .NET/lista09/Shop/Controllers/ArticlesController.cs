using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shop.Models;

namespace Shop.Controllers;

public class ArticlesController : Controller {
    
    private readonly IArticlesContext _context;

    public ArticlesController(IArticlesContext context) {
        _context = context;
    }

    public IActionResult Index() {
        var articles = _context.GetAll();
        ViewData["message"] = TempData["message"];
        return View(articles);
    }

    public IActionResult Create() {
        ViewData["categories"] = Enum.GetValues<Category>();
        return View();
    }

    public IActionResult Edit(int id) {
        ViewData["categories"] = Enum.GetValues<Category>();
        ViewData["article"] = _context.GetById(id);
        return View();
    }

    [HttpPost]
    public IActionResult AddArticle(Article article) {
        var message = _context.Add(article) 
            ? new Message("Successfully added new article.", false) 
            : new Message("Could not add article.", true);

        TempData["message"] = JsonSerializer.Serialize(message);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditArticle(int id, Article article) {
        var message = _context.Update(id, article) 
            ? new Message("Successfully updated article.", false) 
            : new Message("Could not update article.", true);

        TempData["message"] = JsonSerializer.Serialize(message);

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id) {
        var message = _context.Delete(id) 
            ? new Message("Successfully deleted article.", false) 
            : new Message("Could not delete article.", true);

        TempData["message"] = JsonSerializer.Serialize(message);
        
        return RedirectToAction("Index");
    }
}