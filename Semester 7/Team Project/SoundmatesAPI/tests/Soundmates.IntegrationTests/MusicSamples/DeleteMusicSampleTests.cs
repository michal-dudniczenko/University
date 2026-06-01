using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Text;

namespace Soundmates.IntegrationTests.MusicSamples;

/// <summary>
/// Integration tests for DELETE /music-samples/{musicSampleId} (DeleteMusicSample).
/// Covers sections 3.34, CC-AUTH-1..6, CC-GA-1..4, CC-CSRF-1..4 from tests-plan.md.
/// </summary>
public sealed class DeleteMusicSampleTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // =========================================================================
    // Helpers
    // =========================================================================

    private static readonly CompositeFormat DeleteRouteFormat =
        CompositeFormat.Parse(MusicSamplesTestConstants.DeleteRouteTemplate);

    private static Uri DeleteRoute(Guid sampleId) =>
        new(string.Format(CultureInfo.InvariantCulture, DeleteRouteFormat, sampleId), UriKind.Relative);

    // =========================================================================
    // H1 — sample belongs to caller → 200; row removed from DB
    //      (seeded samples have no real file on disk, so the handler skips the File.Delete
    //       call and proceeds directly to the ExecuteDeleteAsync — this is the E1 path too)
    // =========================================================================

    [Fact]
    public async Task Delete_OwnSample_ReturnsOkAndRowRemoved()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "my-sample.mp3", displayOrder: 0);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var exists = await db.MusicSamples.AnyAsync(ms => ms.Id == sampleId);
            exists.Should().BeFalse("the row must be deleted after a successful Delete call");
        });
    }

    // =========================================================================
    // E1 — DB row exists but file is already missing on disk → skips delete,
    //       still removes the row → 200
    //       This is the natural case for seeded test data (seeding only creates a DB row).
    // =========================================================================

    [Fact]
    public async Task Delete_SampleFileAlreadyMissingOnDisk_ReturnsOkAndRemovesRow()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        // Seed with a filename that does not exist on disk (no real file was uploaded).
        var sampleId = await Factory.SeedMusicSampleAsync(
            user.Id, "file-that-does-not-exist.mp3", displayOrder: 0);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "missing file on disk must not prevent the DB row from being removed");

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var exists = await db.MusicSamples.AnyAsync(ms => ms.Id == sampleId);
            exists.Should().BeFalse("the DB row must still be cleaned up even when the file is absent");
        });
    }

    // =========================================================================
    // V1 — musicSampleId route param is not a GUID → 422
    // =========================================================================

    [Fact]
    public async Task Delete_NonGuidRouteParam_Returns422()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(
            new Uri(string.Format(CultureInfo.InvariantCulture, DeleteRouteFormat,
                MusicSamplesTestConstants.NonGuidId), UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // =========================================================================
    // F1 — sample not found → 404
    // =========================================================================

    [Fact]
    public async Task Delete_NonexistentSampleId_Returns404()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // =========================================================================
    // F2 — sample belongs to another user → 401
    // =========================================================================

    [Fact]
    public async Task Delete_SampleBelongingToAnotherUser_Returns401()
    {
        var ownerUser = await Factory.CreateOnboardedArtistAsync();
        var callerUser = await Factory.CreateOnboardedArtistAsync();

        // Seed the sample under ownerUser.
        var sampleId = await Factory.SeedMusicSampleAsync(
            ownerUser.Id, "owner-sample.mp3", displayOrder: 0);

        // Attempt deletion as callerUser (a different user).
        var callerClient = await Factory.CreateAuthenticatedClientAsync(callerUser);
        var response = await callerClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Row must still exist — callerUser must not be able to delete it.
        await Factory.ExecuteDbContextAsync(async db =>
        {
            var exists = await db.MusicSamples.AnyAsync(ms => ms.Id == sampleId);
            exists.Should().BeTrue("the sample must not be deleted when the caller is not the owner");
        });
    }

    // =========================================================================
    // CC-CSRF-1 — cookie auth without X-CSRF-TOKEN header → 400
    // =========================================================================

    [Fact]
    public async Task Delete_CookieAuth_WithoutCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "csrf-test.mp3", displayOrder: 0);
        var cookieClientNoCsrf = await Factory.CreateCookieClientAsync(user, attachCsrf: false);

        var response = await cookieClientNoCsrf.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // CC-CSRF-2 — cookie auth with invalid CSRF token → 400
    // =========================================================================

    [Fact]
    public async Task Delete_CookieAuth_WithInvalidCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "csrf-bad.mp3", displayOrder: 0);
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: false);
        cookieClient.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bad-csrf-value");

        var response = await cookieClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // CC-CSRF-3 — cookie auth with valid CSRF token → reaches handler (200)
    // =========================================================================

    [Fact]
    public async Task Delete_CookieAuth_WithValidCsrfToken_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "csrf-good.mp3", displayOrder: 0);
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await cookieClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-CSRF-4 — JWT auth → CSRF skipped; reaches handler (200)
    // =========================================================================

    [Fact]
    public async Task Delete_JwtAuth_CsrfSkipped_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "jwt-no-csrf.mp3", displayOrder: 0);
        // JWT client never sends a CSRF token — must still succeed.
        var jwtClient = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await jwtClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-AUTH-1 — no credentials → 401
    // =========================================================================

    [Fact]
    public async Task Delete_NoCredentials_Returns401()
    {
        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-2 — malformed Bearer token → 401
    // =========================================================================

    [Fact]
    public async Task Delete_MalformedBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("not.a.valid.jwt");

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-3 — expired JWT → 401
    // =========================================================================

    [Fact]
    public async Task Delete_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var expiredToken = await Factory.MintExpiredTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(expiredToken);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4a — JWT signed with wrong key → 401
    // =========================================================================

    [Fact]
    public async Task Delete_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4b — JWT with wrong issuer → 401
    // =========================================================================

    [Fact]
    public async Task Delete_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4c — JWT with wrong audience → 401
    // =========================================================================

    [Fact]
    public async Task Delete_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-5 — invalid/expired auth cookie → 401
    // =========================================================================

    [Fact]
    public async Task Delete_InvalidAuthCookie_Returns401()
    {
        var client = Factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid-cookie");

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-6 — both JWT and cookie auth modes reach the handler
    // =========================================================================

    [Fact]
    public async Task Delete_JwtAuthMode_ReachesHandler()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "auth6-jwt.mp3", displayOrder: 0);
        var jwtClient = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await jwtClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "JWT bearer auth must reach the handler");
    }

    [Fact]
    public async Task Delete_CookieAuthMode_ReachesHandler()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "auth6-cookie.mp3", displayOrder: 0);
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await cookieClient.DeleteAsync(DeleteRoute(sampleId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "Cookie auth must reach the handler when CSRF token is valid");
    }

    // =========================================================================
    // CC-GA-1 — valid token but user row deleted → 401
    // =========================================================================

    [Fact]
    public async Task Delete_TokenForNonexistentUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-2 — unconfirmed email → 401
    // =========================================================================

    [Fact]
    public async Task Delete_UnconfirmedUser_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-3 — deactivated user → 401
    // =========================================================================

    [Fact]
    public async Task Delete_DeactivatedUser_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-4 — first-login user (IsFirstLogin = true) → 401
    // =========================================================================

    [Fact]
    public async Task Delete_FirstLoginUser_Returns401()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
