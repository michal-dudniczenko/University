using Soundmates.IntegrationTests.Auth;
using System.Net;
using System.Text;

namespace Soundmates.IntegrationTests.Common.Csrf;

public sealed class CsrfTokenFilterTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri LogoutRouteUri = new(AuthTestConstants.LogoutRoute, UriKind.Relative);
    private static readonly Uri ChangePasswordRouteUri = new(AuthTestConstants.ChangePasswordRoute, UriKind.Relative);

    [Fact]
    public async Task CsrfFilter_CookieAuthWithoutToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: false);

        var response = await client.PostAsync(LogoutRouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.ReadStringAsync()).Should().Contain("CSRF");
    }

    [Fact]
    public async Task CsrfFilter_CookieAuthWithInvalidToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "invalid-csrf-token");

        var response = await client.PostAsync(LogoutRouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CsrfFilter_CookieAuthWithValidToken_PassesThrough()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await client.PostAsync(LogoutRouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CsrfFilter_JwtAuth_SkipsCheck()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(LogoutRouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CsrfFilter_RunsBeforeValidation_Returns400NotValidationProblem()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: false);

        // Send an empty body to an endpoint with both CSRF filter and ValidationFilter.
        // CSRF runs first so we get 400 "CSRF" rather than 422 from the validator.
        using var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = await client.PostAsync(ChangePasswordRouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.ReadStringAsync()).Should().Contain("CSRF");
    }
}
