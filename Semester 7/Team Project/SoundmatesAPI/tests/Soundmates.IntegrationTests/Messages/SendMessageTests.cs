using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Messages.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Soundmates.IntegrationTests.Messages;

/// <summary>
/// Tests for POST /messages — SendMessage (3.29 in tests-plan.md).
///
/// Notable behaviours exercised here:
///  - The CSRF filter is attached BEFORE the validation filter, so for a cookie request missing the
///    CSRF token a 400 wins over a 422 body error (E1 / filter ordering).
///  - A missing match yields 401 (the unusual status choice — not 403/404).
///  - Receiver IsFirstLogin is NOT checked here (unlike like/dislike); only IsActive + EmailConfirmed.
/// </summary>
public sealed class SendMessageTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(MessagesTestConstants.SendMessageRoute, UriKind.Relative);

    private static Task<HttpResponseMessage> PostAsync(HttpClient client, SendMessageRequest request) =>
        client.PostAsJsonAsync(RouteUri, request, TestJson.Options, TestContext.Current.CancellationToken);

    private async Task<(TestUser caller, TestUser receiver, HttpClient client)> SeedMatchedPairAsync()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        return (caller, receiver, client);
    }

    // -------------------------------------------------------------------------
    // H1 — matched receiver + valid content → 200; Message row (IsSeen=false),
    //      SignalR "MessageReceived" to receiver group { senderId, senderName }.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_MatchedReceiverValidContent_ReturnsOkPersistsAndNotifies()
    {
        var caller = await Factory.CreateOnboardedArtistAsync(name: "Sender Sam");
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var receiverToken = await Factory.GetAccessTokenAsync(receiver.Id);
        await using var hub = await EventHubTestClient.ConnectAsync(
            Factory, receiverToken, MessagesTestConstants.MessageReceivedEvent);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // SignalR payload shape.
        var payload = await hub.WaitForEventAsync(MessagesTestConstants.MessageReceivedEvent);
        payload.GetProperty("senderId").GetGuid().Should().Be(caller.Id);
        payload.GetProperty("senderName").GetString().Should().Be("Sender Sam");

        // DB side-effect: exactly one message, unseen, with the right participants and content.
        await Factory.ExecuteDbContextAsync(async db =>
        {
            var message = await db.Messages.AsNoTracking()
                .SingleAsync(m => m.SenderId == caller.Id && m.ReceiverId == receiver.Id);
            message.Content.Should().Be(MessagesTestConstants.DefaultContent);
            message.IsSeen.Should().BeFalse();
        });
    }

    // -------------------------------------------------------------------------
    // V1 — ReceiverId empty / not a GUID → 422
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_ReceiverIdEmpty_Returns422()
    {
        var (_, _, client) = await SeedMatchedPairAsync();

        var response = await PostAsync(client,
            new SendMessageRequest(string.Empty, MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SendMessage_ReceiverIdNotAGuid_Returns422()
    {
        var (_, _, client) = await SeedMatchedPairAsync();

        var response = await PostAsync(client,
            new SendMessageRequest("not-a-guid", MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // -------------------------------------------------------------------------
    // V2 — Content empty → 422
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_ContentEmpty_Returns422()
    {
        var (_, receiver, client) = await SeedMatchedPairAsync();

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), string.Empty));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // -------------------------------------------------------------------------
    // V3 — Content length > 4000 → 422; boundary exactly 4000 → pass.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_ContentTooLong_Returns422()
    {
        var (_, receiver, client) = await SeedMatchedPairAsync();
        var tooLong = new string('a', MessagesTestConstants.MaxContentLength + 1);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), tooLong));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SendMessage_ContentExactlyMaxLength_ReturnsOk()
    {
        var (_, receiver, client) = await SeedMatchedPairAsync();
        var atLimit = new string('a', MessagesTestConstants.MaxContentLength);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), atLimit));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // F1 — receiverId == caller.Id → 400 "cannot send message to yourself".
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_ToSelf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await PostAsync(client,
            new SendMessageRequest(caller.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // F2 — Receiver nonexistent / inactive / unconfirmed → 404.
    //      (IsFirstLogin is NOT checked here, unlike like/dislike.)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_ReceiverNonexistent_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await PostAsync(client,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SendMessage_ReceiverInactive_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateDeactivatedUserAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SendMessage_ReceiverUnconfirmed_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateUnconfirmedUserAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------------------
    // F3 — No match between caller and receiver → 401 (unusual status — assert it is 401).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_NoMatch_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // E1 — Filter ordering: cookie request missing CSRF AND invalid body → 400 (not 422),
    //      because the CSRF filter runs BEFORE the validation filter.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_CookieMissingCsrfAndInvalidBody_Returns400NotValidation()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        // Body is also invalid (empty receiver + empty content) — but CSRF (400) must win.
        var response = await PostAsync(client, new SendMessageRequest(string.Empty, string.Empty));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF-1 — cookie request without X-CSRF-TOKEN header (valid body) → 400.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_CookieWithoutCsrfToken_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF-2 — cookie request with an invalid / mismatched CSRF token → 400.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_CookieWithInvalidCsrfToken_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);

        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "totally-invalid-token");

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF-3 — cookie request with a valid CSRF token pair → passes CSRF, reaches handler (200).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_CookieWithValidCsrfToken_ReturnsOk()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var receiver = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, receiver.Id);
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF-4 — JWT request → CSRF skipped (no token required), reaches handler (200).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_JwtRequest_SkipsCsrfAndReachesHandler()
    {
        var (_, receiver, client) = await SeedMatchedPairAsync();

        var response = await PostAsync(client,
            new SendMessageRequest(receiver.Id.ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH-1..6 — framework authentication layer.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_NoCredentials_Returns401()
    {
        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_GarbageBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("this-is-not-a-jwt");

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintExpiredTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_InvalidAuthCookie_Returns401()
    {
        // No login performed; attach a bogus auth cookie value.
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid-cookie-value");

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true default).
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_TokenForNonexistentUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        var token = await Factory.GetAccessTokenAsync(caller.Id);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        var token = await Factory.GetAccessTokenAsync(caller.Id);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SendMessage_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        var token = await Factory.GetAccessTokenAsync(caller.Id);
        HttpClient.SetBearerToken(token);

        var response = await PostAsync(HttpClient,
            new SendMessageRequest(Guid.NewGuid().ToString(), MessagesTestConstants.DefaultContent));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-VAL-3/4/5 — body binding failures.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_MalformedJson_Returns400()
    {
        var (_, _, client) = await SeedMatchedPairAsync();
        using var content = new StringContent("{ not valid json ", Encoding.UTF8, "application/json");

        var response = await client.PostAsync(RouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendMessage_WrongContentType_Returns415()
    {
        var (_, receiver, client) = await SeedMatchedPairAsync();
        using var content = new StringContent(
            $"{{\"receiverId\":\"{receiver.Id}\",\"content\":\"hi\"}}", Encoding.UTF8, "text/plain");

        var response = await client.PostAsync(RouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    // -------------------------------------------------------------------------
    // CC-ROUTE-2 — correct route, wrong HTTP method → 405.
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SendMessage_WrongHttpMethod_Returns405()
    {
        var (_, _, client) = await SeedMatchedPairAsync();

        var response = await client.PutAsync(RouteUri, JsonContent.Create(
            new SendMessageRequest(Guid.NewGuid().ToString(), "hi"), options: TestJson.Options),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}
