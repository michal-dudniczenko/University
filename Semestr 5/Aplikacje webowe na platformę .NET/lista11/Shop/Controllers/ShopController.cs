using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers;

public class ShopController : Controller
{

    private readonly ShopContext _context;

    public ShopController(ShopContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {

        var categories = await _context.Category.ToListAsync();
        ViewData["Categories"] = categories;

        var articles = categoryId.HasValue ?
            _context.Article.Include(a => a.Category).Where(a => a.CategoryId == categoryId) :
            _context.Article.Include(a => a.Category);

        ViewData["SelectedCategoryId"] = categoryId;
        return View(await articles.ToListAsync());
    }

    public IActionResult AddToCart(int articleId, bool isFromCart = false)
    {
        if (Request.Cookies.TryGetValue($"article{articleId}", out var value))
        {
            Response.Cookies.Append($"article{articleId}", (int.Parse(value) + 1).ToString(),
                new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }
        else
        {
            Response.Cookies.Append($"article{articleId}", "1",
                new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }

        if (isFromCart)
        {
            return RedirectToAction(nameof(ViewCart));
        } else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult RemoveFromCart(int articleId)
    {
        if (Request.Cookies.TryGetValue($"article{articleId}", out var value))
        {
            if (int.Parse(value) > 1)
            {
                Response.Cookies.Append($"article{articleId}", (int.Parse(value) - 1).ToString(),
                    new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            }
            else
            {
                Response.Cookies.Delete($"article{articleId}");
            }
        }

        return RedirectToAction(nameof(ViewCart));
    }

    public IActionResult RemoveAllFromCart(int articleId)
    {
        Response.Cookies.Delete($"article{articleId}");

        return RedirectToAction(nameof(ViewCart));
    }

    public IActionResult ViewCart()
    {
        var cartItems = Request.Cookies
            .Where(c => c.Key.StartsWith("article"))
            .Select(c => new
            {
                Article = _context.Article
                    .Include(a => a.Category)
                    .FirstOrDefault(a => a.Id == int.Parse(c.Key.Replace("article", ""))),
                Quantity = int.Parse(c.Value)
            })
            .Where(x => x.Article != null) // Exclude null articles
            .OrderBy(x => x?.Article?.Name)
            .ToDictionary(x => x.Article!, x => x.Quantity);

        return View(cartItems);
    }
}