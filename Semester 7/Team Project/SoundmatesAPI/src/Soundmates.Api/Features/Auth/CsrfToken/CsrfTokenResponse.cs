namespace Soundmates.Api.Features.Auth.CsrfToken;

internal sealed record CsrfTokenResponse(string Token, string HeaderName, string CookieName);
