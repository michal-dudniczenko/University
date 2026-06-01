using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for POST /matching/dislike — CreateDislike (3.21 in tests-plan.md).
/// Authenticated · ValidationFilter&lt;CreateDislikeRequest&gt; · CSRF · GetAuthorizedUserAsync(true).
/// </summary>
public sealed class CreateDislikeTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

    // H1 — valid receiver, no prior reaction → 200; Dislike row; never a match.
    [Fact]
    public async Task CreateDislike_ValidReceiver_AddsDislikeNeverMatch()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();

        // Even a reciprocal like must not produce a match for a dislike.
        await Factory.SeedLikeAsync(receiver.Id, caller.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            (await db.Dislikes.AnyAsync(d => d.GiverId == caller.Id && d.ReceiverId == receiver.Id))
                .Should().BeTrue("the dislike row must be persisted");
            (await db.Matches.AnyAsync()).Should().BeFalse("a dislike never creates a match");
        });
    }

    // -------------------------------------------------------------------------
    // Validation (CC-VAL)
    // -------------------------------------------------------------------------

    // V1a — ReceiverId empty → 422.
    [Fact]
    public async Task CreateDislike_EmptyReceiverId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(string.Empty), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V1b — ReceiverId not a GUID → 422.
    [Fact]
    public async Task CreateDislike_NonGuidReceiverId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest("nope"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-VAL-3 — empty body → 400.
    [Fact]
    public async Task CreateDislike_EmptyBody_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new StringContent("", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-VAL-4 — malformed JSON → 400.
    [Fact]
    public async Task CreateDislike_MalformedJson_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new StringContent("{bad", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-VAL-5 — wrong Content-Type → 415.
    [Fact]
    public async Task CreateDislike_WrongContentType_Returns415()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new StringContent("x", Encoding.UTF8, "text/plain"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    // -------------------------------------------------------------------------
    // Handler failures / edge
    // -------------------------------------------------------------------------

    // F1 — disliking yourself → 400.
    [Fact]
    public async Task CreateDislike_SelfReceiver_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(caller.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // F2 — receiver nonexistent → 404.
    [Fact]
    public async Task CreateDislike_NonexistentReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver inactive → 404.
    [Fact]
    public async Task CreateDislike_InactiveReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver unconfirmed → 404.
    [Fact]
    public async Task CreateDislike_UnconfirmedReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F2 — receiver first-login → 404.
    [Fact]
    public async Task CreateDislike_FirstLoginReceiver_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // F3 — existing like toward receiver → 400.
    [Fact]
    public async Task CreateDislike_AlreadyLiked_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedLikeAsync(caller.Id, receiver.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // F4 — existing dislike toward receiver → 400.
    [Fact]
    public async Task CreateDislike_AlreadyDisliked_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedDislikeAsync(caller.Id, receiver.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateDislike_NoCredentials_Returns401()
    {
        var receiver = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_GarbageBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintExpiredTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_WrongKeyToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_JwtAndCookieBothReachHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var r1 = await Factory.CreateOnboardedArtistAsync();
        var r2 = await Factory.CreateOnboardedArtistAsync();

        var jwtClient = await Factory.CreateAuthenticatedClientAsync(caller);
        (await jwtClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(r1.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var cookieClient = await Factory.CreateCookieClientAsync(caller);
        (await cookieClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(r2.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateDislike_TokenForDeletedUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateDislike_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        var token = await Factory.MintTokenAsync(caller.Id, caller.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(Guid.NewGuid().ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateDislike_CookieAuthMissingCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDislike_CookieAuthInvalidCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bogus");

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDislike_CookieAuthValidCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDislike_JwtAuthSkipsCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), new CreateDislikeRequest(receiver.Id.ToString()), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // Routing
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateDislike_GetToPostRoute_Returns405()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(new Uri(MatchingTestConstants.DislikeRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}
