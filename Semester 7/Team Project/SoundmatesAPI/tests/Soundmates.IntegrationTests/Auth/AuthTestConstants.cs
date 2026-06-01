namespace Soundmates.IntegrationTests.Auth;

/// <summary>Reusable magic strings/values for the Auth domain tests.</summary>
internal static class AuthTestConstants
{
    // Routes.
    public const string RegisterRoute = "/auth/register";
    public const string LoginRoute = "/auth/login";
    public const string ConfirmEmailRoute = "/auth/confirm-email";
    public const string ResendEmailRoute = "/auth/resend-email";
    public const string ForgotPasswordRoute = "/auth/forgot-password";
    public const string ResetPasswordRoute = "/auth/reset-password";
    public const string ChangePasswordRoute = "/auth/change-password";
    public const string DeactivateRoute = "/auth/deactivate";
    public const string LogoutRoute = "/auth/logout";
    public const string RefreshRoute = "/auth/refresh";
    public const string RevokeTokenRoute = "/auth/token/revoke";
    public const string RevokeAllTokensRoute = "/auth/token/revoke-all";
    public const string CsrfTokenRoute = "/auth/csrf-token";

    // A fixed IP for rate-limit scenarios (shares one fixed-window bucket).
    public const string RateLimitIp = "203.0.113.10";

    // Number of requests permitted by the auth fixed-window rate limiter per minute.
    public const int RateLimitPermitCount = 10;

    // A new valid password distinct from TestConstants.DefaultPassword for change/reset flows.
    public const string NewValidPassword = "NewPassw0rd!";

    // RULE-PASSWORD boundary/failure values.
    public const string ValidPasswordMin8 = "Abcd123!";              // exactly 8 chars, valid
    public const string ValidPasswordMax32 = "Abcdefghijklmnopqrstuv012345678!"; // exactly 32 chars, valid
    public const string PasswordTooShort = "Ab1!def";                // 7 chars
    public const string PasswordTooLong = "Abcdefghijklmnopqrstuvwxyz012345!"; // 33 chars
    public const string PasswordNoLower = "ABCD123!";
    public const string PasswordNoUpper = "abcd123!";
    public const string PasswordNoDigit = "Abcdefg!";
    public const string PasswordNoSpecial = "Abcd1234";
    public const string PasswordWithSpace = "Abc d12!";              // contains a space (outside 33-126)
    public const string PasswordWithAccent = "Abcd12!é";             // accented char outside ASCII 33-126

    // RULE-EMAIL invalid variants. FluentValidation's default EmailAddress() (AspNetCoreCompatible)
    // only requires an '@' that is not the first/last char, so we use values that reliably fail:
    // no '@' at all, and '@' at the trailing edge.
    public const string EmailInvalidNoAt = "notanemail";
    public const string EmailInvalidTrailingAt = "foo@";
    public const string EmailInvalidLeadingAt = "@example.com";
}
