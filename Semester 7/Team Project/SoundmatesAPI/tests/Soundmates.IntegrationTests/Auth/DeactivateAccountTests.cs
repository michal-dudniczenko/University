using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class DeactivateAccountTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.DeactivateRoute, UriKind.Relative);
    private static readonly Uri LoginRouteUri = new($"{AuthTestConstants.LoginRoute}?useCookies=false", UriKind.Relative);

    private static DeactivateAccountRequest ValidRequest() => new(TestConstants.DefaultPassword);

    [Fact]
    public async Task Deactivate_CorrectPassword_DeactivatesUserAndRevokesTokens()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedRefreshTokenAsync(user.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var (isActive, deactivatedAt, refreshCount) = await Factory.ExecuteDbContextAsync(async db =>
        {
            var u = await db.Users.AsNoTracking().FirstAsync(x => x.Id == user.Id);
            var count = await db.RefreshTokens.CountAsync(rt => rt.UserId == user.Id);
            return (u.IsActive, u.DeactivatedAt, count);
        });

        isActive.Should().BeFalse();
        deactivatedAt.Should().NotBeNull();
        refreshCount.Should().Be(0);
    }

    [Fact]
    public async Task Deactivate_EmptyPassword_Returns422()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(
            RouteUri, new DeactivateAccountRequest(""), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task Deactivate_WrongPassword_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(
            RouteUri, new DeactivateAccountRequest("WrongPassw0rd!"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Deactivate_FirstLoginUser_Returns204()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            "DeactivateAccount uses checkForFirstLogin:false");
    }

    [Fact]
    public async Task Deactivate_ThenLogin_Returns403()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var deactivate = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var login = await HttpClient.PostAsJsonAsync(
            LoginRouteUri,
            new LoginRequest(user.Email, TestConstants.DefaultPassword),
            TestJson.Options,
            TestContext.Current.CancellationToken);
        login.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Deactivate_UnconfirmedCaller_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Deactivate_DeactivatedCaller_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Deactivate_TokenForNonexistentUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Deactivate_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync(RouteUri, ValidRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
