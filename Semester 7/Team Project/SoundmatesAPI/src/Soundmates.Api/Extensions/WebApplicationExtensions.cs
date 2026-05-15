using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Extensions;

internal static class WebApplicationExtensions
{
    public static async Task InitializeMigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
