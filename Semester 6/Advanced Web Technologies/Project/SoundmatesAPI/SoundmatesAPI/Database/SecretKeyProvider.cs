namespace SoundmatesAPI.Database;

public class SecretKeyProvider : ISecretKeyProvider
{
    private readonly string _secretKey;

    public SecretKeyProvider(AppDbContext context)
    {
        var secret = context.Secrets.OrderBy(s => s.SecretKey).FirstOrDefault();
        if (secret == null || string.IsNullOrEmpty(secret.SecretKey))
        {
            throw new InvalidOperationException("Secret key not found in the database.");
        }

        _secretKey = secret.SecretKey;
    }

    public string GetSecretKey() => _secretKey;
}
