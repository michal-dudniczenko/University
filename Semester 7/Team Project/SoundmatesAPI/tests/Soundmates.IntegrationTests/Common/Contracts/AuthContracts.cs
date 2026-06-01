namespace Soundmates.IntegrationTests.Common.Contracts;

// Duplicated request/response DTOs used by the shared test foundation. Per project rules the
// test project never references the API's internal DTO records; it mirrors their JSON shape.

internal sealed record RegisterRequest(string Email, string Password);

internal sealed record LoginRequest(string Email, string Password);

internal sealed record LoginResponse(string AccessToken, string RefreshToken);

internal sealed record RefreshRequest(string RefreshToken);

internal sealed record RefreshResponse(string AccessToken, string RefreshToken);

internal sealed record CsrfTokenResponse(string Token, string HeaderName, string CookieName);
