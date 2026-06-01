using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Common.Auth;

internal static class AuthenticationExtensions
{
    private static readonly Uri LoginUri = new("/auth/login?useCookies=true", UriKind.Relative);
    private static readonly Uri CsrfTokenUri = new("/auth/csrf-token", UriKind.Relative);

    /// <summary>Generates a real access token for a seeded user via the app's IAuthService.</summary>
    public static Task<string> GetAccessTokenAsync(this CustomWebApplicationFactory factory, Guid userId) =>
        factory.ExecuteScopeAsync(async sp =>
        {
            var userManager = sp.GetRequiredService<UserManager<User>>();
            var authService = sp.GetRequiredService<IAuthService>();

            var user = await userManager.FindByIdAsync(userId.ToString())
                ?? throw new InvalidOperationException($"No seeded user with id {userId}.");
            return await authService.GenerateAccessTokenAsync(user);
        });

    public static void SetBearerToken(this HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    /// <summary>New https client authenticated as <paramref name="user"/> via JWT bearer.</summary>
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        this CustomWebApplicationFactory factory, TestUser user, string? remoteIp = null)
    {
        var client = factory.CreateApiClient(remoteIp);
        var token = await factory.GetAccessTokenAsync(user.Id);
        client.SetBearerToken(token);
        return client;
    }

    /// <summary>
    /// New https client carrying a validly-signed JWT for a non-persisted user. The global fallback
    /// policy requires an authenticated user on EVERY request — including ones that don't match any
    /// endpoint — so unauthenticated requests get 401 before routing can produce 404 (unknown route),
    /// 405 (wrong method) or 415 (unsupported media type). JWT validation never hits the database, so
    /// a token for a random id is enough to satisfy the fallback and let those status codes surface.
    /// Use only where the request is rejected before any handler reads the user.
    /// </summary>
    public static async Task<HttpClient> CreateRoutingProbeClientAsync(this CustomWebApplicationFactory factory)
    {
        var client = factory.CreateApiClient();
        client.SetBearerToken(await factory.MintTokenAsync(Guid.NewGuid(), "routing-probe@test.local"));
        return client;
    }

    /// <summary>Logs in with cookie auth (asserts 200). The auth cookie is stored on the client.</summary>
    public static async Task LoginWithCookiesAsync(this HttpClient client, TestUser user)
    {
        var response = await client.PostAsJsonAsync(
            LoginUri,
            new LoginRequest(user.Email, user.Password),
            TestJson.Options,
            CancellationToken.None);
        response.IsSuccessStatusCode.Should().BeTrue(
            "cookie login should succeed for a seeded user, got {0}", response.StatusCode);
    }

    /// <summary>GETs /auth/csrf-token (stores the XSRF cookie) and returns the request token.</summary>
    public static async Task<string> FetchCsrfTokenAsync(this HttpClient client)
    {
        var response = await client.GetAsync(CsrfTokenUri, CancellationToken.None);
        var body = await response.ReadRequiredAsync<CsrfTokenResponse>();
        return body.Token;
    }

    /// <summary>
    /// New https client with a cookie session for <paramref name="user"/>. When
    /// <paramref name="attachCsrf"/> is true the X-CSRF-TOKEN header is set by default so mutations
    /// pass the CSRF filter; set false to test the missing-token path.
    /// </summary>
    public static async Task<HttpClient> CreateCookieClientAsync(
        this CustomWebApplicationFactory factory, TestUser user, bool attachCsrf = true)
    {
        var client = factory.CreateApiClient();
        await client.LoginWithCookiesAsync(user);

        var token = await client.FetchCsrfTokenAsync();
        if (attachCsrf)
        {
            client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, token);
        }

        return client;
    }
}
