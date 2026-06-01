using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Messages.Contracts;
using System.Data.SqlTypes;
using System.Net;

namespace Soundmates.IntegrationTests.Messages;

/// <summary>
/// Tests for GET /messages/{otherUserId}?limit&amp;offset — GetConversation (3.30 in tests-plan.md).
///
/// Notable behaviours exercised here:
///  - NO match is required to fetch a conversation (contrast SendMessage / ViewConversation).
///  - The other user is only filtered on !IsFirstLogin — inactive / unconfirmed counterparts still
///    return their history; nonexistent / first-login counterparts → 404.
///  - Self-id is allowed (no self-conversation guard, unlike ViewConversation).
///  - Validation order: route GUID is checked before pagination.
///  - Ordering is CreatedAt desc then Id asc.
/// </summary>
public sealed class GetConversationTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri RouteWithPaging(object otherUserId, int limit = 50, int offset = 0) =>
        new($"{MessagesTestConstants.ConversationRoute(otherUserId)}?limit={limit}&offset={offset}", UriKind.Relative);

    // -------------------------------------------------------------------------
    // H1 — messages both directions, ordered by CreatedAt desc then Id, paginated
    //      each MessageResponse { Content, Timestamp, SenderId, ReceiverId, IsSeen }.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_BothDirections_ReturnsOrderedMessages()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();

        var baseTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        await Factory.SeedMessageAsync(caller.Id, other.Id, "first", createdAt: baseTime);
        await Factory.SeedMessageAsync(other.Id, caller.Id, "second", createdAt: baseTime.AddMinutes(1));
        await Factory.SeedMessageAsync(caller.Id, other.Id, "third", createdAt: baseTime.AddMinutes(2));

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();

        messages.Should().HaveCount(3);
        messages.Select(m => m.Content).Should().ContainInConsecutiveOrder("third", "second", "first");
        messages.Should().AllSatisfy(m =>
        {
            (m.SenderId == caller.Id || m.ReceiverId == caller.Id).Should().BeTrue();
        });
    }

    [Fact]
    public async Task GetConversation_SameCreatedAt_TieBreaksByIdAscending()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();

        var ts = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var firstId = await Factory.SeedMessageAsync(caller.Id, other.Id, "A", createdAt: ts);
        var secondId = await Factory.SeedMessageAsync(other.Id, caller.Id, "B", createdAt: ts);
        secondId.Should().NotBe(firstId);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();

        // The endpoint tie-breaks equal CreatedAt with ThenBy(Id), translated to SQL `ORDER BY Id`.
        // SQL Server orders uniqueidentifier differently from .NET/GUID-v7 ordering, so the lower
        // value is decided by SqlGuid (which mirrors SQL Server's ordering), not by which was seeded
        // first.
        var expectedOrder = new SqlGuid(firstId).CompareTo(new SqlGuid(secondId)) < 0
            ? new[] { "A", "B" }
            : new[] { "B", "A" };
        messages.Select(m => m.Content).Should().ContainInConsecutiveOrder(expectedOrder);
    }

    // -------------------------------------------------------------------------
    // V1 — otherUserId not a GUID → 422 (route GUID validated, checked before pagination).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_OtherUserIdNotAGuid_Returns422()
    {
        var client = await Factory.CreateAuthenticatedClientAsync(
            await Factory.CreateOnboardedArtistAsync());

        var response = await client.GetAsync(RouteWithPaging("not-a-guid"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetConversation_InvalidGuidAndInvalidPagination_GuidWins422()
    {
        // GUID is validated before pagination; with both invalid we still get 422 (not 400 binding,
        // since the values still bind as ints — limit=0/offset=-1 are valid ints).
        var client = await Factory.CreateAuthenticatedClientAsync(
            await Factory.CreateOnboardedArtistAsync());

        var response = await client.GetAsync(
            new Uri($"{MessagesTestConstants.ConversationRoute("not-a-guid")}?limit=0&offset=-1", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // -------------------------------------------------------------------------
    // V2 / CC-PAG-1..6 — pagination (with a valid GUID).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_LimitZero_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id, limit: 0), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetConversation_LimitAboveMax_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id, limit: 51), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetConversation_NegativeOffset_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id, offset: -1), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetConversation_LimitAndOffsetBothInvalid_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id, limit: 0, offset: -1), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(50, 0)]
    public async Task GetConversation_ValidBoundaries_ReturnsOk(int limit, int offset)
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id, limit, offset), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetConversation_NonIntegerLimit_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(
            new Uri($"{MessagesTestConstants.ConversationRoute(other.Id)}?limit=abc&offset=0", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConversation_Paginated_ReturnsStablePages()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var baseTime = new DateTime(2025, 2, 1, 9, 0, 0, DateTimeKind.Utc);
        for (var i = 0; i < 5; i++)
        {
            await Factory.SeedMessageAsync(caller.Id, other.Id, $"msg{i}", createdAt: baseTime.AddMinutes(i));
        }

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var page1 = await (await client.GetAsync(RouteWithPaging(other.Id, limit: 2, offset: 0), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<MessageResponse>>();
        var page2 = await (await client.GetAsync(RouteWithPaging(other.Id, limit: 2, offset: 2), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<MessageResponse>>();

        page1.Should().HaveCount(2);
        page2.Should().HaveCount(2);
        // Newest first: msg4, msg3 | msg2, msg1 ...
        page1.Select(m => m.Content).Should().ContainInConsecutiveOrder("msg4", "msg3");
        page2.Select(m => m.Content).Should().ContainInConsecutiveOrder("msg2", "msg1");
    }

    // -------------------------------------------------------------------------
    // F1 — other user nonexistent OR IsFirstLogin → 404 (filter is !u.IsFirstLogin).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_OtherUserNonexistent_Returns404()
    {
        var client = await Factory.CreateAuthenticatedClientAsync(
            await Factory.CreateOnboardedArtistAsync());

        var response = await client.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetConversation_OtherUserFirstLogin_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // E1 — No match required: fetching with a non-first-login user without a match works.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_NoMatch_StillReturnsHistory()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        // No match seeded; just messages.
        await Factory.SeedMessageAsync(caller.Id, other.Id, "no-match-msg");
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();
        messages.Should().ContainSingle(m => m.Content == "no-match-msg");
    }

    // -------------------------------------------------------------------------
    // E2 — No messages between the pair → empty list (but 200).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_NoMessages_ReturnsEmptyList()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();
        messages.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // E3 — Self-id allowed → 200 with self-sent messages (no self-conversation guard).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_SelfId_ReturnsOk()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMessageAsync(caller.Id, caller.Id, "note-to-self");
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(caller.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();
        messages.Should().ContainSingle(m => m.Content == "note-to-self");
    }

    // -------------------------------------------------------------------------
    // E4 — Inactive / unconfirmed other user still returns history (only IsFirstLogin filtered).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_InactiveOtherUser_ReturnsHistory()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateDeactivatedUserAsync();
        await Factory.SeedMessageAsync(other.Id, caller.Id, "from-inactive");
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();
        messages.Should().ContainSingle(m => m.Content == "from-inactive");
    }

    [Fact]
    public async Task GetConversation_UnconfirmedOtherUser_ReturnsHistory()
    {
        // An unconfirmed user is created with IsFirstLogin=true by the seeder, which would 404.
        // Flip IsFirstLogin to false directly so we isolate the unconfirmed condition.
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateUnconfirmedUserAsync();
        await Factory.ExecuteDbContextAsync(async db =>
        {
            await db.Users.Where(u => u.Id == other.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsFirstLogin, false));
        });
        await Factory.SeedMessageAsync(other.Id, caller.Id, "from-unconfirmed");
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await response.ReadRequiredAsync<List<MessageResponse>>();
        messages.Should().ContainSingle(m => m.Content == "from-unconfirmed");
    }

    // -------------------------------------------------------------------------
    // CC-AUTH-1..6 — framework authentication layer.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_GarbageBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("not-a-jwt");

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongKeyTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_ValidCookieScheme_ReachesHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        // GET has no CSRF filter; cookie scheme must reach the handler (200, empty list).
        var response = await client.GetAsync(RouteWithPaging(other.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true default).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetConversation_TokenForNonexistentUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversation_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteWithPaging(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
