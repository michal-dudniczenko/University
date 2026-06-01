using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class RevokeTokenTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.RevokeTokenRoute, UriKind.Relative);
    private static readonly Uri RefreshRouteUri = new(AuthTestConstants.RefreshRoute, UriKind.Relative);

    [Fact]
    public async Task RevokeToken_ExistingToken_Returns204DeletesRowAndBlocksRefresh()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RevokeTokenRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(0, "the matching refresh token row is deleted");

        var refresh = await HttpClient.PostAsJsonAsync(
            RefreshRouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);
        refresh.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeToken_EmptyToken_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RevokeTokenRequest(""), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("RefreshToken");
    }

    [Fact]
    public async Task RevokeToken_UnknownToken_Returns204AsIdempotentNoop()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RevokeTokenRequest("no-such-token"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent,
            "ExecuteDelete of 0 rows is an idempotent no-op");
    }

    [Fact]
    public async Task RevokeToken_OnlySuppliedTokenIsRevoked()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var rawToRevoke = await Factory.SeedRefreshTokenAsync(user.Id);
        var rawToKeep = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RevokeTokenRequest(rawToRevoke), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(1, "only the supplied token is revoked");

        var refresh = await HttpClient.PostAsJsonAsync(
            RefreshRouteUri, new RefreshRequest(rawToKeep), TestJson.Options, TestContext.Current.CancellationToken);
        refresh.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
