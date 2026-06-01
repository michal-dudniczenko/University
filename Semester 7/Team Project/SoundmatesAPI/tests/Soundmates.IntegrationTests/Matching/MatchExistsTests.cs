using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for GET /matching/match/exists/{userId} — MatchExists (3.26 in tests-plan.md).
/// Authenticated · GetAuthorizedUserAsync(true) · route GUID via GuidValidator.
/// </summary>
public sealed class MatchExistsTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri Route(object userId) => new($"{MatchingTestConstants.MatchExistsRoute}/{userId}", UriKind.Relative);

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — match exists, caller is User2 (other is User1) → true.
    [Fact]
    public async Task MatchExists_MatchPresentCallerIsUser2_ReturnsTrue()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        // User1Id = other, User2Id = caller
        await Factory.SeedMatchAsync(other.Id, caller.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<bool>()).Should().BeTrue();
    }

    // H1 — match exists, caller is User1 (other is User2) → true (both positions checked).
    [Fact]
    public async Task MatchExists_MatchPresentCallerIsUser1_ReturnsTrue()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        // User1Id = caller, User2Id = other
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<bool>()).Should().BeTrue();
    }

    // H1 — both participants see the match (query is order-agnostic).
    [Fact]
    public async Task MatchExists_BothParticipantsSeeMatch_ReturnsTrue()
    {
        var a = await Factory.CreateOnboardedArtistAsync();
        var b = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(a.Id, b.Id);

        var aClient = await Factory.CreateAuthenticatedClientAsync(a);
        (await (await aClient.GetAsync(Route(b.Id), TestContext.Current.CancellationToken)).ReadRequiredAsync<bool>()).Should().BeTrue();

        var bClient = await Factory.CreateAuthenticatedClientAsync(b);
        (await (await bClient.GetAsync(Route(a.Id), TestContext.Current.CancellationToken)).ReadRequiredAsync<bool>()).Should().BeTrue();
    }

    // H2 — no match → false.
    [Fact]
    public async Task MatchExists_NoMatch_ReturnsFalse()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<bool>()).Should().BeFalse();
    }

    // E1 — well-formed but nonexistent target → false (existence not required).
    [Fact]
    public async Task MatchExists_NonexistentTarget_ReturnsFalse()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<bool>()).Should().BeFalse();
    }

    // -------------------------------------------------------------------------
    // Validation / handler failures
    // -------------------------------------------------------------------------

    // V1 — userId route param not a GUID → 422 (key "fieldName").
    [Fact]
    public async Task MatchExists_NonGuidRouteParam_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route("not-a-guid"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // F1 — userId == caller.Id → 400.
    [Fact]
    public async Task MatchExists_SelfTarget_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route(caller.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MatchExists_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_WrongIssuerToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_CookieAuthReachesHandler_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.GetAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task MatchExists_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MatchExists_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
