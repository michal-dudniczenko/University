namespace Soundmates.Api.Features.Users.ChangePassword;

internal sealed record ChangePasswordRequest(string OldPassword, string NewPassword);
