namespace Soundmates.IntegrationTests.Auth.Contracts;

// Local mirrors of the API's Auth request DTOs. Per project rules the test project never
// references the API's internal DTO records; it mirrors their JSON shape here. The shared
// Common.Contracts types (RegisterRequest, LoginRequest, LoginResponse, RefreshRequest,
// RefreshResponse, CsrfTokenResponse) are NOT redefined and are used directly.

internal sealed record ConfirmEmailRequest(string Token);

internal sealed record ResendEmailConfirmationRequest(string Email);

internal sealed record ForgotPasswordRequest(string Email);

internal sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

internal sealed record ChangePasswordRequest(string OldPassword, string NewPassword);

internal sealed record DeactivateAccountRequest(string Password);

internal sealed record RevokeTokenRequest(string RefreshToken);
