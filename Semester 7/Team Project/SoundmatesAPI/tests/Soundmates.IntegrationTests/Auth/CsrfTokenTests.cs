using System.Net;

namespace Soundmates.IntegrationTests.Auth;

public sealed class CsrfTokenTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.CsrfTokenRoute, UriKind.Relative);

    [Fact]
    public async Task CsrfToken_Get_ReturnsTokenWithCookieAndNoCacheHeaders()
    {
        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.ReadRequiredAsync<CsrfTokenResponse>();
        body.Token.Should().NotBeNullOrEmpty();
        body.HeaderName.Should().Be(TestConstants.CsrfTokenHeaderName);
        body.CookieName.Should().Be(TestConstants.CsrfTokenCookieName);

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        string.Join(";", cookies!).Should().Contain(TestConstants.CsrfTokenCookieName);

        response.Headers.CacheControl!.NoStore.Should().BeTrue("Cache-Control must be no-store");
        response.Headers.Pragma.ToString().Should().Contain("no-cache");
    }
}
