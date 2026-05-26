namespace Soundmates.Api.Features.Auth.Login;

internal sealed record LoginResponse(string AccessToken, string RefreshToken);
