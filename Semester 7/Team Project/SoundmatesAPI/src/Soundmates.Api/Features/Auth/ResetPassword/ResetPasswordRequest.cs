namespace Soundmates.Api.Features.Auth.ResetPassword;

internal sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);
