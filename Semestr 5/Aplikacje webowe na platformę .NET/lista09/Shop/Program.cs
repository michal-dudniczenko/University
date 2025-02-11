using Shop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// builder.Services.AddSingleton<IArticlesContext, ArticlesListContext>();
builder.Services.AddSingleton<IArticlesContext, ArticlesDictionaryContext>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}")
    .WithStaticAssets();
    

app.Run();
