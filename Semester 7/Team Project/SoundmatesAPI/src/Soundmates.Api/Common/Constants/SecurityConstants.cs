namespace Soundmates.Api.Common.Constants;

internal static class SecurityConstants
{
    public const string CsrfTokenHeaderName = "X-CSRF-TOKEN";
    public const string CsrfTokenCookieName = "XSRF-TOKEN";

    public const int MinimumPasswordLength = 8;
    public const int MaximumPasswordLength = 32;

    public const string CustomAuthPolicyName = "BearerOrCookie";
    public const string CustomAuthPolicyDescription = "JWT Bearer or Cookie";

    public const string AuthCookieName = "auth";

    public const string PolicyRequireAdmin = "RequireAdmin";
    public const string PolicyRequireEmailConfirmed = "RequireEmailConfirmed";

    public const string RateLimitingAuthPolicyName = "authRateLimit";

    public const string ConfirmEmailEndpointClientPath = "https://localhost:5555/confirm-email";
    public const string ResetPasswordEndpointClientPath = "https://localhost:5555/reset-password";

    public const int ConfirmEmailExpireDurationMinutes = 30;
}
