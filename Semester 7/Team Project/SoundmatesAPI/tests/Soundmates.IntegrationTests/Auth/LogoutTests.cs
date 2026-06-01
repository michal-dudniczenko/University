using System.Net;

namespace Soundmates.IntegrationTests.Auth;

public sealed class LogoutTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.LogoutRoute, UriKind.Relative);

    [Fact]
    public async Task Logout_CookieSession_Returns204AndClearsAuthCookie()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // SignOutAsync emits a Set-Cookie that expires the auth cookie.
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue(
            "sign-out must emit a Set-Cookie clearing the auth cookie");
        string.Join(";", cookies!).Should().Contain(TestConstants.AuthCookieName);
    }

    [Fact]
    public async Task Logout_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_JwtAuth_Returns204AsNoop()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            "JWT logout reaches the handler but cookie sign-out has nothing to clear");
    }
}
