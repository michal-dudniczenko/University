using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Auth.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Auth;

public sealed class RefreshTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(AuthTestConstants.RefreshRoute, UriKind.Relative);

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokensAndRotates()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<RefreshResponse>();
        body.AccessToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBe(raw, "the refresh token is rotated");

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(1, "old token row deleted, new one inserted");
    }

    [Fact]
    public async Task Refresh_EmptyToken_Returns422()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(""), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey("RefreshToken");
    }

    [Fact]
    public async Task Refresh_UnknownToken_Returns401()
    {
        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest("no-such-refresh-token"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ExpiredToken_Returns401AndDeletesRow()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var raw = await Factory.SeedRefreshTokenAsync(
            user.Id, expiresAt: DateTime.UtcNow.AddDays(-1));

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(0, "the expired row is deleted");
    }

    [Fact]
    public async Task Refresh_InactiveUser_Returns401AndDeletesRow()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(0);
    }

    [Fact]
    public async Task Refresh_UnconfirmedUser_Returns401AndDeletesRow()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var count = await Factory.ExecuteDbContextAsync(db =>
            db.RefreshTokens.AsNoTracking().CountAsync(rt => rt.UserId == user.Id));
        count.Should().Be(0);
    }

    [Fact]
    public async Task Refresh_ReusedTokenAfterRotation_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var first = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);
        second.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_NewAccessToken_ContainsRequiredClaims()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var raw = await Factory.SeedRefreshTokenAsync(user.Id);

        var response = await HttpClient.PostAsJsonAsync(
            RouteUri, new RefreshRequest(raw), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.ReadRequiredAsync<RefreshResponse>();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(body.AccessToken);

        jwt.Claims.Should().Contain(c =>
            c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        jwt.Claims.Should().Contain(c =>
            c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Name);
        jwt.ValidTo.Should().BeAfter(DateTime.UtcNow, "the access token must have a fresh expiry");
    }
}
