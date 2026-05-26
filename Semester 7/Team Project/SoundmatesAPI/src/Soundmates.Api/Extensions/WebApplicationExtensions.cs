using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Options;
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

    public static async Task SeedApplicationAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var adminUserOptions = services.GetRequiredService<IOptions<AdminUserOptions>>().Value;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        if (!await roleManager.RoleExistsAsync(AdminUserOptions.AdminRoleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(AdminUserOptions.AdminRoleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin role. Errors: {errors}");
            }
        }

        var admin = await userManager.FindByEmailAsync(adminUserOptions.Email);

        if (admin is null)
        {
            admin = new User
            {
                Email = adminUserOptions.Email,
                UserName = adminUserOptions.Email,
                EmailConfirmed = true,
                IsFirstLogin = false
            };

            var result = await userManager.CreateAsync(admin, adminUserOptions.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user. Errors: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminUserOptions.AdminRoleName))
        {
            var result = await userManager.AddToRoleAsync(admin, AdminUserOptions.AdminRoleName);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign admin role to admin user. Errors: {errors}");
            }
        }
    }
}
