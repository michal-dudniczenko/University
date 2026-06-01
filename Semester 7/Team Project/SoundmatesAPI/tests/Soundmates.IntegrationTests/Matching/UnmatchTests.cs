using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for DELETE /matching/unmatch/{userId} — Unmatch (3.28 in tests-plan.md).
/// Authenticated · CSRF · GetAuthorizedUserAsync(true) · route GUID via GuidValidator.
/// </summary>
public sealed class UnmatchTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri Route(object userId) => new($"{MatchingTestConstants.UnmatchRoute}/{userId}", UriKind.Relative);

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — caller is User1 → 200; match deleted.
    [Fact]
    public async Task Unmatch_CallerIsUser1_DeletesMatch()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await Factory.ExecuteDbContextAsync(async db =>
            (await db.Matches.AnyAsync()).Should().BeFalse("the match row must be deleted"));
    }

    // H1 — caller is User2 → 200; match deleted regardless of stored order.
    [Fact]
    public async Task Unmatch_CallerIsUser2_DeletesMatch()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(other.Id, caller.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await Factory.ExecuteDbContextAsync(async db =>
            (await db.Matches.AnyAsync()).Should().BeFalse("the match row must be deleted"));
    }

    // E1 — existing messages between the pair survive the unmatch (orphaned conversation).
    [Fact]
    public async Task Unmatch_MessagesSurvive()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        await Factory.SeedMessageAsync(caller.Id, other.Id, "hi");
        await Factory.SeedMessageAsync(other.Id, caller.Id, "hello back");

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            (await db.Matches.AnyAsync()).Should().BeFalse();
            (await db.Messages.CountAsync(m =>
                (m.SenderId == caller.Id && m.ReceiverId == other.Id) ||
                (m.SenderId == other.Id && m.ReceiverId == caller.Id)))
                .Should().Be(2, "unmatch must not delete the conversation messages");
        });
    }

    // -------------------------------------------------------------------------
    // Validation / handler failures
    // -------------------------------------------------------------------------

    // V1 — userId not a GUID → 422.
    [Fact]
    public async Task Unmatch_NonGuidRouteParam_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.DeleteAsync(Route("not-a-guid"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // F1 — unmatch yourself → 400.
    [Fact]
    public async Task Unmatch_SelfTarget_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.DeleteAsync(Route(caller.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // F2 — no match with the target → 404.
    [Fact]
    public async Task Unmatch_NoMatch_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Unmatch_NoCredentials_Returns401()
    {
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_WrongKeyToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongKeyTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_JwtAndCookieBothReachHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var o1 = await Factory.CreateOnboardedArtistAsync();
        var o2 = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, o1.Id);
        await Factory.SeedMatchAsync(caller.Id, o2.Id);

        var jwtClient = await Factory.CreateAuthenticatedClientAsync(caller);
        (await jwtClient.DeleteAsync(Route(o1.Id), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);

        var cookieClient = await Factory.CreateCookieClientAsync(caller);
        (await cookieClient.DeleteAsync(Route(o2.Id), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task Unmatch_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unmatch_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.DeleteAsync(Route(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF
    // -------------------------------------------------------------------------

    // CC-CSRF-1 — cookie auth, no CSRF header → 400.
    [Fact]
    public async Task Unmatch_CookieAuthMissingCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-CSRF-2 — cookie auth, invalid CSRF token → 400.
    [Fact]
    public async Task Unmatch_CookieAuthInvalidCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bogus");

        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-CSRF-3 — cookie auth, valid CSRF token → 200.
    [Fact]
    public async Task Unmatch_CookieAuthValidCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // CC-CSRF-4 — JWT auth → CSRF skipped (covered by H1; asserted here explicitly).
    [Fact]
    public async Task Unmatch_JwtAuthSkipsCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.DeleteAsync(Route(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
