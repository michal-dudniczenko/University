using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.Auth;

public sealed class RevokeAllTokensTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.RevokeAllTokensRoute, UriKind.Relative);

    private Task<int> RefreshTokenCountAsync(Guid userId) =>
        Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == userId));

    [Fact]
    public async Task RevokeAllTokens_MultipleTokens_DeletesAll()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedRefreshTokenAsync(user.Id);
        await Factory.SeedRefreshTokenAsync(user.Id);
        await Factory.SeedRefreshTokenAsync(user.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await RefreshTokenCountAsync(user.Id)).Should().Be(0, "all of the caller's tokens are deleted");
    }

    [Fact]
    public async Task RevokeAllTokens_FirstLoginUser_Returns204()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        await Factory.SeedRefreshTokenAsync(user.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            "RevokeAllTokens uses checkForFirstLogin: false, so first-login users are allowed");
        (await RefreshTokenCountAsync(user.Id)).Should().Be(0);
    }

    [Fact]
    public async Task RevokeAllTokens_NoExistingTokens_Returns204AsNoop()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RevokeAllTokens_OnlyRemovesCallersTokens()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedRefreshTokenAsync(caller.Id);
        await Factory.SeedRefreshTokenAsync(other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await RefreshTokenCountAsync(caller.Id)).Should().Be(0);
        (await RefreshTokenCountAsync(other.Id)).Should().Be(1, "another user's tokens are untouched");
    }

    [Fact]
    public async Task RevokeAllTokens_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeAllTokens_TokenForNonexistentUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeAllTokens_UnconfirmedCaller_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeAllTokens_DeactivatedCaller_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(RouteUri, null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
