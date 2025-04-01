using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop.Controllers;

public class ShopController : Controller {

    private readonly ShopContext _context;

    public ShopController(ShopContext context) {
        _context = context;
    }
    
    public async Task<IActionResult> Index(int? categoryId) {

        var categories = await _context.Category.ToListAsync();
        ViewData["Categories"] = categories;

        var articles = categoryId.HasValue ? 
            _context.Article.Include(a => a.Category).Where(a => a.CategoryId == categoryId) :
            _context.Article.Include(a => a.Category);
        
        ViewData["SelectedCategoryId"] = categoryId;
        return View(await articles.ToListAsync());
    }
}