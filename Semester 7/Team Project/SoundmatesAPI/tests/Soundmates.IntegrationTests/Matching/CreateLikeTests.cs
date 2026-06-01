using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for POST /matching/like — CreateLike (3.20 in tests-plan.md).
/// Authenticated · ValidationFilter&lt;CreateLikeRequest&gt; · CSRF · GetAuthorizedUserAsync(true).
/// </summary>
public sealed class CreateLikeTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — valid receiver, no prior reaction, no reciprocal like → 200; Like row; no Match; no events.
    [Fact]
    public async Task CreateLike_NoReciprocalLike_AddsLikeWithoutMatchOrEvents()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();

        var callerToken = await Factory.GetAccessTokenAsync(caller.Id);
        await using var hub = await EventHubTestClient.ConnectAsync(
            Factory, callerToken, MatchingTestConstants.MatchReceivedEvent, MatchingTestConstants.MatchCreatedEvent);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            (await db.Likes.AnyAsync(l => l.GiverId == caller.Id && l.ReceiverId == receiver.Id))
                .Should().BeTrue("the like row must be persisted");
            (await db.Matches.AnyAsync()).Should().BeFalse("no reciprocal like exists, so no match is created");
        });

        (await hub.NoEventReceivedAsync(MatchingTestConstants.MatchCreatedEvent))
            .Should().BeTrue("a non-reciprocal like must not emit MatchCreated");
    }

    // H2 / E3 — reciprocal like → 200; Like + Match created; SignalR MatchReceived + MatchCreated with documented payloads.
    [Fact]
    public async Task CreateLike_ReciprocalLike_CreatesMatchAndSendsEvents()
    {
        const string callerName = "Caller Artist";
        var caller = await Factory.CreateOnboardedArtistAsync(name: callerName);
        var receiver = await Factory.CreateOnboardedArtistAsync();

        // Receiver already liked the caller.
        await Factory.SeedLikeAsync(receiver.Id, caller.Id);

        var receiverToken = await Factory.GetAccessTokenAsync(receiver.Id);
        var callerToken = await Factory.GetAccessTokenAsync(caller.Id);

        await using var receiverHub = await EventHubTestClient.ConnectAsync(
            Factory, receiverToken, MatchingTestConstants.MatchReceivedEvent);
        await using var callerHub = await EventHubTestClient.ConnectAsync(
            Factory, callerToken, MatchingTestConstants.MatchCreatedEvent);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            (await db.Likes.AnyAsync(l => l.GiverId == caller.Id && l.ReceiverId == receiver.Id))
                .Should().BeTrue();
            (await db.Matches.AnyAsync(m => m.User1Id == caller.Id && m.User2Id == receiver.Id))
                .Should().BeTrue("match is created with User1Id=caller (the liker-back), User2Id=receiver (the original liker)");
        });

        // MatchReceived → receiver group: { newLikeUserId, newLikeUserName }
        var received = await receiverHub.WaitForEventAsync(MatchingTestConstants.MatchReceivedEvent);
        received.GetProperty("newLikeUserId").GetGuid().Should().Be(caller.Id);
        received.GetProperty("newLikeUserName").GetString().Should().Be(callerName);

        // MatchCreated → caller group: { existingLikeUserId }
        var created = await callerHub.WaitForEventAsync(MatchingTestConstants.MatchCreatedEvent);
        created.GetProperty("existingLikeUserId").GetGuid().Should().Be(receiver.Id);
    }

    // -------------------------------------------------------------------------
    // Validation (CC-VAL)
    // -------------------------------------------------------------------------

    // CC-VAL-1 — valid body reaches the handler (covered by H1).

    // V1a — ReceiverId empty → 422 (NotEmpty fires).
    [Fact]
    public async Task CreateLike_EmptyReceiverId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(string.Empty), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V1b — ReceiverId not a GUID → 422 (ValidGuid fires).
    [Fact]
    public async Task CreateLike_NonGuidReceiverId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest("not-a-guid"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-VAL-3 — missing/empty body → 400 (framework binding).
    [Fact]
    public async Task CreateLike_EmptyBody_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new StringContent("", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-VAL-4 — malformed JSON → 400.
    [Fact]
    public async Task CreateLike_MalformedJson_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new StringContent("{ not json", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-VAL-5 — wrong Content-Type → 415.
    [Fact]
    public async Task CreateLike_WrongContentType_Returns415()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new StringContent(receiver.Id.ToString(), Encoding.UTF8, "text/plain"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    // -------------------------------------------------------------------------
    // Handler failures / edge
    // -------------------------------------------------------------------------

    // F1 — liking yourself → 400.
    [Fact]
    public async Task CreateLike_SelfReceiver_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(caller.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // F2 — receiver does not exist → 404.
    [Fact]
    public async Task CreateLike_NonexistentReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver inactive → 404.
    [Fact]
    public async Task CreateLike_InactiveReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver unconfirmed → 404.
    [Fact]
    public async Task CreateLike_UnconfirmedReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver first-login → 404.
    [Fact]
    public async Task CreateLike_FirstLoginReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F3 — caller already liked the receiver → 400.
    [Fact]
    public async Task CreateLike_AlreadyLiked_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedLikeAsync(caller.Id, receiver.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // F4 — caller already disliked the receiver → 400 (one reaction per pair).
    [Fact]
    public async Task CreateLike_AlreadyDisliked_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedDislikeAsync(caller.Id, receiver.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // E2 — after a match exists, liking again is blocked by F3 (already liked).
    [Fact]
    public async Task CreateLike_AfterMatch_LikingAgainReturns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedLikeAsync(receiver.Id, caller.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var first = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    // CC-AUTH-1 — no credentials → 401.
    [Fact]
    public async Task CreateLike_NoCredentials_Returns401()
    {
        var receiver = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-2 — garbage bearer token → 401.
    [Fact]
    public async Task CreateLike_GarbageBearerToken_Returns401()
    {
        var receiver = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken("not.a.jwt");

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-3 — expired JWT → 401.
    [Fact]
    public async Task CreateLike_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintExpiredTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-4 — wrong key / issuer / audience → 401.
    [Fact]
    public async Task CreateLike_WrongKeyToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateLike_WrongIssuerToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongIssuerTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateLike_WrongAudienceToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongAudienceTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-5 — invalid auth cookie → 401.
    [Fact]
    public async Task CreateLike_InvalidAuthCookie_Returns401()
    {
        var receiver = await Factory.CreateOnboardedArtistAsync();
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid-cookie-value");

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-6 — JWT and cookie scheme both reach the handler.
    [Fact]
    public async Task CreateLike_JwtAndCookieBothReachHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var r1 = await Factory.CreateOnboardedArtistAsync();
        var r2 = await Factory.CreateOnboardedArtistAsync();

        var jwtClient = await Factory.CreateAuthenticatedClientAsync(caller);
        var jwtResponse = await jwtClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(r1.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);
        jwtResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var cookieClient = await Factory.CreateCookieClientAsync(caller);
        var cookieResponse = await cookieClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(r2.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);
        cookieResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA (GetAuthorizedUserAsync gate, checkForFirstLogin: true)
    // -------------------------------------------------------------------------

    // CC-GA-1 — token for a user that no longer exists → 401.
    [Fact]
    public async Task CreateLike_TokenForDeletedUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-GA-2 — unconfirmed caller → 401.
    [Fact]
    public async Task CreateLike_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-GA-3 — deactivated caller → 401.
    [Fact]
    public async Task CreateLike_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-GA-4 — first-login caller → 401 (checkForFirstLogin: true).
    [Fact]
    public async Task CreateLike_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF
    // -------------------------------------------------------------------------

    // CC-CSRF-1 — cookie auth, no X-CSRF-TOKEN header → 400.
    [Fact]
    public async Task CreateLike_CookieAuthMissingCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-CSRF-2 — cookie auth, invalid CSRF token → 400.
    [Fact]
    public async Task CreateLike_CookieAuthInvalidCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bogus-token-value");

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-CSRF-3 — cookie auth, valid CSRF token → reaches handler (200).
    [Fact]
    public async Task CreateLike_CookieAuthValidCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // CC-CSRF-4 — JWT auth → CSRF skipped, reaches handler (covered by H1, asserted explicitly here).
    [Fact]
    public async Task CreateLike_JwtAuthSkipsCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), new CreateLikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // Routing
    // -------------------------------------------------------------------------

    // CC-ROUTE-2 — wrong HTTP method → 405.
    [Fact]
    public async Task CreateLike_GetToPostRoute_Returns405()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(new Uri(MatchingTestConstants.LikeRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}
