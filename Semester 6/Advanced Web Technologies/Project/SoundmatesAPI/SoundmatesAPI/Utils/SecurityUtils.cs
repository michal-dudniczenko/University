using System.Security.Cryptography;
using System.Text;

namespace SoundmatesAPI.Security;

public static class SecurityUtils
{
    public static string ComputeTokenHash(string data, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public static string GenerateRefreshToken(int size = 32)
    {
        var randomBytes = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public static string GenerateAccessToken(Guid userId, string secretKey)
    {
        string userIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
        string expirationIso = DateTime.UtcNow.AddMinutes(10).ToString("o"); // ISO 8601
        string expirationBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(expirationIso));

        string dataToSign = $"{userIdBase64}.{expirationBase64}";
        string hash = ComputeTokenHash(dataToSign, secretKey);

        return $"{userIdBase64}.{expirationBase64}.{hash}";
    }

    public static Guid? VerifyAccessToken(string token, string secretKey)
    {
        Guid userId;

        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        string userIdBase64 = parts[0];
        string expirationBase64 = parts[1];
        string providedHash = parts[2];

        try
        {
            string userIdStr = Encoding.UTF8.GetString(Convert.FromBase64String(userIdBase64));
            string expirationIso = Encoding.UTF8.GetString(Convert.FromBase64String(expirationBase64));

            if (!Guid.TryParse(userIdStr, out userId))
                return null;

            if (!DateTime.TryParse(expirationIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expirationDate))
                return null;

            if (expirationDate < DateTime.UtcNow)
                return null;

            string dataToSign = $"{userIdBase64}.{expirationBase64}";
            string expectedHash = ComputeTokenHash(dataToSign, secretKey);

            if (expectedHash == providedHash)
            {
                return userId;
            } else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    public static string GenerateSecret(int length = 32)
    {
        const string allowedChars =
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "0123456789" +
            "!@#$%^&*()-_=+[]{}<>?/";

        return string.Concat(Enumerable.Range(0, length).Select(_ =>
        {
            var index = RandomNumberGenerator.GetInt32(allowedChars.Length);
            return allowedChars[index];
        }));
    }
}


