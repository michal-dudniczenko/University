using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.Messages;

/// <summary>
/// Tests for POST /messages/{otherUserId}/view — ViewConversation (3.32 in tests-plan.md).
///
/// Notable behaviours exercised here:
///  - A match IS required (no match → 401).
///  - Other user is filtered on IsActive + EmailConfirmed (nonexistent/inactive/unconfirmed → 404),
///    but IsFirstLogin is NOT checked (a first-login counterpart still → 200) — asymmetry vs
///    GetConversation.
///  - Only incoming (other → caller) unseen messages are flipped to seen.
///  - The "ConversationSeen" SignalR payload's "userId" MUST equal the CALLER's id (the person who
///    read the conversation). The current handler sends otherUserGuid instead — the userId
///    assertions below intentionally assert the CORRECT value (caller's id) so they fail against
///    that bug, per the plan's explicit verification note (3.32 H1).
/// </summary>
public sealed class ViewConversationTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private async Task<(TestUser caller, TestUser other, HttpClient client)> SeedMatchedPairAsync()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        return (caller, other, client);
    }

    private static Task<HttpResponseMessage> ViewAsync(HttpClient client, object otherUserId) =>
        client.PostAsync(
            new Uri(MessagesTestConstants.ViewConversationRoute(otherUserId), UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

    // -------------------------------------------------------------------------
    // H1 — incoming unseen messages flipped to seen + "ConversationSeen" with userId == caller.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_Matched_FlipsIncomingUnseenAndNotifiesWithCallerId()
    {
        var (caller, other, client) = await SeedMatchedPairAsync();

        // Two unseen incoming messages (other → caller).
        var incoming1 = await Factory.SeedMessageAsync(other.Id, caller.Id, "in1", isSeen: false);
        var incoming2 = await Factory.SeedMessageAsync(other.Id, caller.Id, "in2", isSeen: false);

        var otherToken = await Factory.GetAccessTokenAsync(other.Id);
        await using var hub = await EventHubTestClient.ConnectAsync(
            Factory, otherToken, MessagesTestConstants.ConversationSeenEvent);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await hub.WaitForEventAsync(MessagesTestConstants.ConversationSeenEvent);
        payload.TryGetProperty("timestamp", out _).Should().BeTrue("payload must carry a timestamp");
        // userId MUST be the caller's id (who read the conversation), NOT the other user's id.
        payload.GetProperty("userId").GetGuid().Should().Be(caller.Id,
            "the ConversationSeen payload must identify the caller as the reader");

        // Both incoming messages are now seen.
        await Factory.ExecuteDbContextAsync(async db =>
        {
            var seenStates = await db.Messages.AsNoTracking()
                .Where(m => m.Id == incoming1 || m.Id == incoming2)
                .Select(m => m.IsSeen)
                .ToListAsync();
            seenStates.Should().AllSatisfy(s => s.Should().BeTrue());
        });
    }

    // -------------------------------------------------------------------------
    // E2 — only incoming (other → caller) flipped; caller's own + already-seen untouched.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_OnlyIncomingUnseenFlipped()
    {
        var (caller, other, client) = await SeedMatchedPairAsync();

        var incomingUnseen = await Factory.SeedMessageAsync(other.Id, caller.Id, "incoming", isSeen: false);
        var incomingSeen = await Factory.SeedMessageAsync(other.Id, caller.Id, "already-seen", isSeen: true);
        var outgoing = await Factory.SeedMessageAsync(caller.Id, other.Id, "outgoing", isSeen: false);

        var response = await ViewAsync(client, other.Id);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var messages = await db.Messages.AsNoTracking()
                .Where(m => m.Id == incomingUnseen || m.Id == incomingSeen || m.Id == outgoing)
                .ToDictionaryAsync(m => m.Id, m => m.IsSeen);

            messages[incomingUnseen].Should().BeTrue("incoming unseen should be flipped");
            messages[incomingSeen].Should().BeTrue("already-seen stays seen");
            messages[outgoing].Should().BeFalse("the caller's own outgoing message must not be flipped");
        });
    }

    // -------------------------------------------------------------------------
    // E1 — no unseen incoming messages → still 200 + notification fired (0 rows updated).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_NoUnseenIncoming_StillOkAndNotifies()
    {
        var (caller, other, client) = await SeedMatchedPairAsync();

        var otherToken = await Factory.GetAccessTokenAsync(other.Id);
        await using var hub = await EventHubTestClient.ConnectAsync(
            Factory, otherToken, MessagesTestConstants.ConversationSeenEvent);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await hub.WaitForEventAsync(MessagesTestConstants.ConversationSeenEvent);
        payload.GetProperty("userId").GetGuid().Should().Be(caller.Id);
    }

    // -------------------------------------------------------------------------
    // V1 — otherUserId not a GUID → 422.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_OtherUserIdNotAGuid_Returns422()
    {
        var client = await Factory.CreateAuthenticatedClientAsync(
            await Factory.CreateOnboardedArtistAsync());

        var response = await ViewAsync(client, "not-a-guid");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // -------------------------------------------------------------------------
    // F1 — otherUserId == caller.Id → 400 "cannot read your own conversation".
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_SelfId_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, caller.Id);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // F2 — other user nonexistent / inactive / unconfirmed → 404.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_OtherUserNonexistent_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ViewConversation_OtherUserInactive_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateDeactivatedUserAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ViewConversation_OtherUserUnconfirmed_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateUnconfirmedUserAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // F3 — no match → 401.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_NoMatch_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // F4 — other user exists but IsFirstLogin == true → 200 (no IsFirstLogin check here).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_OtherUserFirstLogin_ReturnsOk()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        // First-login user is confirmed + active by default (CreateFirstLoginUserAsync), so it
        // passes the IsActive + EmailConfirmed check; IsFirstLogin is not checked here.
        var other = await Factory.CreateFirstLoginUserAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF-1..4 — ValidateCsrfTokenFilter.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_CookieWithoutCsrfToken_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ViewConversation_CookieWithInvalidCsrfToken_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "totally-invalid-token");

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ViewConversation_CookieWithValidCsrfToken_ReturnsOk()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ViewConversation_JwtRequest_SkipsCsrfAndReachesHandler()
    {
        var (_, other, client) = await SeedMatchedPairAsync();

        var response = await ViewAsync(client, other.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH-1..6 — framework authentication layer.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_NoCredentials_Returns401()
    {
        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_GarbageBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("not-a-jwt");

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(user.Id, user.Email));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongKeyTokenAsync(user.Id, user.Email));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true default).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ViewConversation_TokenForNonexistentUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ViewConversation_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await ViewAsync(HttpClient, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
