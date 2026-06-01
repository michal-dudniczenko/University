using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class LoginTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static Uri Route(string query = "") =>
        new($"{AuthTestConstants.LoginRoute}{query}", UriKind.Relative);

    [Fact]
    public async Task Login_ValidCredentialsNoCookies_ReturnsTokensAndPersistsRefreshToken()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<LoginResponse>();
        body.AccessToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();

        var refreshCount = await Factory.ExecuteDbContextAsync(db => db.RefreshTokens
            .AsNoTracking()
            .CountAsync(rt => rt.UserId == user.Id));
        refreshCount.Should().Be(1);
    }

    [Fact]
    public async Task Login_ValidCredentialsWithCookies_SetsAuthCookieAndReturnsEmptyBody()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=true"),
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadStringAsync()).Should().BeEmpty();
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        string.Join(";", cookies!).Should().Contain(TestConstants.AuthCookieName);
    }

    [Fact]
    public async Task Login_MissingUseCookiesParam_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route(), // no useCookies query param
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.ReadStringAsync();
        body.Should().Contain("useCookies");
    }

    [Theory]
    [InlineData("")]
    [InlineData(AuthTestConstants.EmailInvalidNoAt)]
    [InlineData(AuthTestConstants.EmailInvalidTrailingAt)]
    public async Task Login_InvalidEmail_Returns422(string email)
    {
        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest(email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task Login_EmptyPassword_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest("user@test.local", ""),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Login_NonexistentEmail_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest("does-not-exist@test.local", TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_DeactivatedUser_Returns403()
    {
        var user = await Factory.CreateDeactivatedUserAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await response.ReadStringAsync()).Should().Contain("deactivated");
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest(user.Email, "WrongPassw0rd!"),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_AfterFiveFailedAttempts_Returns423()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        // MaxFailedAccessAttempts = 5 and Identity locks out (and returns LockedOut) on the attempt
        // that reaches the limit, so the first 4 failures return 401 and the 5th returns 423.
        for (var i = 0; i < 4; i++)
        {
            var fail = await HttpClient.PostAsJsonAsync(
                Route("?useCookies=false"),
                new LoginRequest(user.Email, "WrongPassw0rd!"),
                TestJson.Options,
                TestContext.Current.CancellationToken);
            fail.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        var locked = await HttpClient.PostAsJsonAsync(
            Route("?useCookies=false"),
            new LoginRequest(user.Email, "WrongPassw0rd!"),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        locked.StatusCode.Should().Be(HttpStatusCode.Locked);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Login_UnconfirmedEmail_Returns401(bool useCookies)
    {
        var user = await Factory.CreateUnconfirmedUserAsync();

        var response = await HttpClient.PostAsJsonAsync(
            Route($"?useCookies={(useCookies ? "true" : "false")}"),
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_MultipleLogins_EachIssuesNewRefreshToken()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        for (var i = 0; i < 3; i++)
        {
            var resp = await HttpClient.PostAsJsonAsync(
                Route("?useCookies=false"),
                new LoginRequest(user.Email, TestConstants.DefaultPassword),
                TestJson.Options,
                TestContext.Current.CancellationToken);
            resp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var count = await Factory.ExecuteDbContextAsync(db => db.RefreshTokens
            .AsNoTracking()
            .CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(3, "each useCookies=false login issues a new refresh token");
    }
}
