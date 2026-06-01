namespace Soundmates.IntegrationTests.Common;

internal static class TestConstants
{
    public const string DbDockerImageTag = "mcr.microsoft.com/mssql/server:2025-CU5-ubuntu-24.04";
    public const string TestDatabaseName = "SoundmatesTests";
    public const string IntegrationTestsCollectionName = "IntegrationTests";

    public static readonly string[] DictionaryTableNames =
    [
        "Countries",
        "Cities",
        "Genders",
        "BandRoles",
        "TagCategories",
        "Tags"
    ];

    // Base address for the test HttpClient. Must be https because auth/CSRF cookies are
    // issued with Secure=Always + SameSite=None, so a CookieContainer will only echo them
    // back over https. It also avoids the UseHttpsRedirection 307 on http requests.
    public const string ClientBaseAddress = "https://localhost";

    // Header read by TestRemoteIpStartupFilter to set HttpContext.Connection.RemoteIpAddress.
    // Each test gets a unique IP so the per-IP auth rate limiter does not bleed across tests
    // rate-limit scenarios pin a single shared IP on purpose.
    public const string RemoteIpHeaderName = "X-Test-Remote-Ip";

    // A strong password that satisfies the Identity policy (>=8 chars, upper, lower, digit,
    // non-alphanumeric) and the RULE-PASSWORD validator (ASCII 33-126, length 8-32).
    public const string DefaultPassword = "Password123!";

    // Seeded admin (created on demand by the seeder because Respawn wipes AspNetUsers/Roles).
    public const string AdminEmail = "admin@soundmates.test";
    public const string AdminPassword = "Admin123!";
    public const string AdminRoleName = "Admin";

    // CSRF / auth cookie names (mirror SecurityConstants in the API).
    public const string CsrfTokenHeaderName = "X-CSRF-TOKEN";
    public const string CsrfTokenCookieName = "XSRF-TOKEN";
    public const string AuthCookieName = "auth";

    // Recipient of moderation report emails (asserted via the capturing email service).
    public const string ModerationEmail = "soundmatesmoderation@gmail.com";

    // Client-facing link prefixes embedded in confirmation / reset emails. Tests parse the
    // "?token=" query value from the captured link.
    public const string ConfirmEmailClientPath = "https://localhost:5555/confirm-email";
    public const string ResetPasswordClientPath = "https://localhost:5555/reset-password";

    // Wrong JWT signing material for negative auth scenarios (CC-AUTH-4).
    public const string WrongJwtSecretKey = "this-is-a-completely-different-secret-key-32+";
    public const string WrongJwtIssuer = "wrong-issuer";
    public const string WrongJwtAudience = "wrong-audience";
}
