namespace Soundmates.Api.Features.Auth.ChangePassword;

internal sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
