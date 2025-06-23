using SoundmatesAPI.Models;
using SoundmatesAPI.Security;

namespace SoundmatesAPI.Database;

public static class DbSeeder
{
    public static void SeedSecrets(AppDbContext context)
    {
        if (!context.Secrets.Any())
        {
            context.Secrets.Add(new Secret
            {
                SecretKey = SecurityUtils.GenerateSecret()
            });

            context.SaveChanges();
        }
    }
}
