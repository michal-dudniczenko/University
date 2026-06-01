using Soundmates.IntegrationTests.Messages.Contracts;
using System.Data.SqlTypes;
using System.Net;

namespace Soundmates.IntegrationTests.Messages;

/// <summary>
/// Tests for GET /messages/preview — GetConversationsPreview (3.31 in tests-plan.md).
///
/// Returns the single latest message per distinct counterpart the caller participates in
/// (sender or receiver), ordered by CreatedAt desc. "Latest" within an unordered pair is the
/// message with no newer one (tie-break by higher Id).
/// </summary>
public sealed class GetConversationsPreviewTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(MessagesTestConstants.ConversationsPreviewRoute, UriKind.Relative);

    // -------------------------------------------------------------------------
    // H1 / E1 — one latest message per counterpart, ordered by CreatedAt desc.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetPreview_MultipleConversations_ReturnsOneLatestPerCounterpartOrdered()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var alice = await Factory.CreateOnboardedArtistAsync();
        var bob = await Factory.CreateOnboardedArtistAsync();

        var baseTime = new DateTime(2025, 3, 1, 8, 0, 0, DateTimeKind.Utc);

        // Conversation with Alice: latest is at +10 min.
        await Factory.SeedMessageAsync(caller.Id, alice.Id, "alice-old", createdAt: baseTime);
        await Factory.SeedMessageAsync(alice.Id, caller.Id, "alice-latest", createdAt: baseTime.AddMinutes(10));

        // Conversation with Bob: latest is at +20 min (newer than Alice's latest).
        await Factory.SeedMessageAsync(caller.Id, bob.Id, "bob-old", createdAt: baseTime.AddMinutes(5));
        await Factory.SeedMessageAsync(bob.Id, caller.Id, "bob-latest", createdAt: baseTime.AddMinutes(20));

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var previews = await response.ReadRequiredAsync<List<MessageResponse>>();

        // Exactly one preview per counterpart.
        previews.Should().HaveCount(2);
        previews.Select(m => m.Content).Should().ContainInConsecutiveOrder("bob-latest", "alice-latest");
    }

    // -------------------------------------------------------------------------
    // H2 — no conversations → empty list.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetPreview_NoConversations_ReturnsEmptyList()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var previews = await response.ReadRequiredAsync<List<MessageResponse>>();
        previews.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // E2 — tie-break: two messages with identical CreatedAt → the higher Id is the latest.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetPreview_SameCreatedAt_PicksHigherIdAsLatest()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();

        var ts = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc);
        var firstId = await Factory.SeedMessageAsync(caller.Id, other.Id, "tie-lower-id", createdAt: ts);
        var secondId = await Factory.SeedMessageAsync(other.Id, caller.Id, "tie-higher-id", createdAt: ts);
        secondId.Should().NotBe(firstId);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var previews = await response.ReadRequiredAsync<List<MessageResponse>>();
        previews.Should().ContainSingle();

        // The endpoint treats the message with the greatest Id as the latest when CreatedAt ties
        // (newer.Id > m.Id, translated to a SQL comparison). SQL Server orders uniqueidentifier
        // differently from .NET/GUID-v7 ordering, so determine the winner with SqlGuid (which mirrors
        // SQL Server's ordering) rather than assuming the later-seeded row wins.
        var expectedLatest = new SqlGuid(secondId).CompareTo(new SqlGuid(firstId)) > 0
            ? "tie-higher-id"
            : "tie-lower-id";
        previews[0].Content.Should().Be(expectedLatest);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH-1..6 — framework authentication layer.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetPreview_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_GarbageBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("not-a-jwt");

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongKeyTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_ValidCookieScheme_ReachesHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true default).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetPreview_TokenForNonexistentUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreview_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.GetAccessTokenAsync(caller.Id));

        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
