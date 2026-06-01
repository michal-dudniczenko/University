using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.MusicSamples;

/// <summary>
/// Integration tests for POST /music-samples (UploadMusicSample).
/// Covers sections 3.33, CC-AUTH-1..6, CC-GA-1..4, CC-CSRF-1..4 from tests-plan.md.
/// </summary>
public sealed class UploadMusicSampleTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri UploadRouteUri = new(MusicSamplesTestConstants.UploadRoute, UriKind.Relative);

    // =========================================================================
    // H1 — audio/mpeg + .mp3 → 200; MusicSample row added with correct DisplayOrder
    // =========================================================================

    [Fact]
    public async Task Upload_ValidMp3_ReturnsOkAndRowAdded()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var count = await db.MusicSamples.CountAsync(ms => ms.UserId == user.Id);
            count.Should().Be(1, "one MP3 was uploaded");

            var sample = await db.MusicSamples.FirstAsync(ms => ms.UserId == user.Id);
            sample.DisplayOrder.Should().Be(0, "first upload has DisplayOrder = 0 (the prior count)");
            sample.FileName.Should().EndWith(".mp3", "extension must be lower-cased and preserved");
        });
    }

    // =========================================================================
    // H2 — video/mp4 + .mp4 → 200; file stored with .mp4 extension
    // =========================================================================

    [Fact]
    public async Task Upload_ValidMp4_ReturnsOkAndRowAdded()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp4(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var sample = await db.MusicSamples.FirstOrDefaultAsync(ms => ms.UserId == user.Id);
            sample.Should().NotBeNull("a MusicSample row must be persisted");
            sample!.FileName.Should().EndWith(".mp4", "MP4 extension must be stored");
        });
    }

    // =========================================================================
    // F1 — disallowed content-type (with allowed extension) → 400
    // =========================================================================

    [Fact]
    public async Task Upload_DisallowedContentType_WithAllowedExtension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Use image/jpeg (disallowed) but .mp3 extension (allowed) — content-type fails first.
        var form = MultipartContentHelper.BuildFileContent(
            new byte[MusicSamplesTestConstants.SmallFileSizeBytes],
            MusicSamplesTestConstants.Mp3FileName,
            MusicSamplesTestConstants.DisallowedContentType);

        var response = await client.PostAsync(UploadRouteUri, form, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // F2 — disallowed extension (with allowed content-type) → 400
    // =========================================================================

    [Fact]
    public async Task Upload_AllowedContentType_WithDisallowedExtension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Use audio/mpeg (allowed) but .txt extension (disallowed).
        var form = MultipartContentHelper.BuildFileContent(
            new byte[MusicSamplesTestConstants.SmallFileSizeBytes],
            MusicSamplesTestConstants.DisallowedExtensionFileName,
            MultipartContentHelper.ValidMp3ContentType);

        var response = await client.PostAsync(UploadRouteUri, form, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // F3a — cross-format mismatch: audio/mpeg + .mp4 → 400
    // =========================================================================

    [Fact]
    public async Task Upload_AudioMpegContentType_WithMp4Extension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // audio/mpeg is valid, .mp4 is valid, but they refer to different formats.
        var form = MultipartContentHelper.BuildFileContent(
            new byte[MusicSamplesTestConstants.SmallFileSizeBytes],
            MusicSamplesTestConstants.Mp4FileName,       // .mp4 extension
            MultipartContentHelper.ValidMp3ContentType); // audio/mpeg content-type

        var response = await client.PostAsync(UploadRouteUri, form, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // F3b — cross-format mismatch: video/mp4 + .mp3 → 400
    // =========================================================================

    [Fact]
    public async Task Upload_VideoMp4ContentType_WithMp3Extension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // video/mp4 is valid, .mp3 is valid, but they refer to different formats.
        var form = MultipartContentHelper.BuildFileContent(
            new byte[MusicSamplesTestConstants.SmallFileSizeBytes],
            MusicSamplesTestConstants.Mp3FileName,       // .mp3 extension
            MultipartContentHelper.ValidMp4ContentType); // video/mp4 content-type

        var response = await client.PostAsync(UploadRouteUri, form, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // F5a — boundary: user with exactly 4 samples uploads 5th → 200 (DisplayOrder = 4)
    // =========================================================================

    [Fact]
    public async Task Upload_FifthSample_WhenFourAlreadyExist_ReturnsOkWithCorrectDisplayOrder()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Pre-create 4 samples via direct DB seeding (no real files needed for the count check).
        for (var i = 0; i < 4; i++)
        {
            await Factory.SeedMusicSampleAsync(user.Id, $"seed_{i}.mp3", displayOrder: i);
        }

        // 5th upload — should succeed.
        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var count = await db.MusicSamples.CountAsync(ms => ms.UserId == user.Id);
            count.Should().Be(5);

            // The newly uploaded sample must have DisplayOrder = 4 (the count at the time of upload).
            var uploaded = await db.MusicSamples
                .Where(ms => ms.UserId == user.Id)
                .OrderByDescending(ms => ms.DisplayOrder)
                .FirstAsync();
            uploaded.DisplayOrder.Should().Be(4);
        });
    }

    // =========================================================================
    // F5b — user already has 5 samples, 6th upload → 400
    // =========================================================================

    [Fact]
    public async Task Upload_SixthSample_WhenFiveAlreadyExist_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Pre-create 5 samples via direct DB seeding.
        for (var i = 0; i < MusicSamplesTestConstants.MaxSamplesCount; i++)
        {
            await Factory.SeedMusicSampleAsync(user.Id, $"seed_{i}.mp3", displayOrder: i);
        }

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // E1 — missing 'file' form field → 400 (framework binding failure)
    // =========================================================================

    [Fact]
    public async Task Upload_MissingFileField_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Post a multipart form with no 'file' part at all.
        var emptyForm = new MultipartFormDataContent();
        var response = await client.PostAsync(UploadRouteUri, emptyForm, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // E2 — extension is lower-cased in stored filename; DisplayOrder increments per upload
    // =========================================================================

    [Fact]
    public async Task Upload_MultipleSamples_StoredFilenameIsLowercasedAndDisplayOrderIncrements()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        // Upload MP3 with an uppercase extension in the submitted filename.
        // MultipartContentHelper.ValidMp3 uses "sample.mp3" but the handler lower-cases it.
        // We exercise DisplayOrder by uploading two samples back to back.
        var firstResponse = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondResponse = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var samples = await db.MusicSamples
                .Where(ms => ms.UserId == user.Id)
                .OrderBy(ms => ms.DisplayOrder)
                .ToListAsync();

            samples.Should().HaveCount(2);
            samples[0].DisplayOrder.Should().Be(0);
            samples[1].DisplayOrder.Should().Be(1);

            // Both filenames must end with a lowercase extension.
            samples.Should().AllSatisfy(s =>
                s.FileName.Should().MatchRegex(@"\.[a-z0-9]+$",
                    "stored filename extension must be lowercase"));
        });
    }

    // =========================================================================
    // E3 — JWT request: CSRF skipped; cookie request without token → 400 (CC-CSRF-1)
    // =========================================================================

    [Fact]
    public async Task Upload_CookieAuth_WithoutCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        // attachCsrf = false → no X-CSRF-TOKEN header on the cookie client.
        var cookieClientNoCsrf = await Factory.CreateCookieClientAsync(user, attachCsrf: false);

        var response = await cookieClientNoCsrf.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_JwtAuth_WithoutCsrfToken_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        // JWT client never sends a CSRF token — CSRF filter must skip for JWT requests.
        var jwtClient = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await jwtClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-CSRF-2 — cookie auth with invalid CSRF token → 400
    // =========================================================================

    [Fact]
    public async Task Upload_CookieAuth_WithInvalidCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: false);
        // Attach a deliberately wrong CSRF token.
        cookieClient.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "invalid-csrf-token");

        var response = await cookieClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =========================================================================
    // CC-CSRF-3 — cookie auth with valid CSRF token → reaches handler (200)
    // =========================================================================

    [Fact]
    public async Task Upload_CookieAuth_WithValidCsrfToken_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        // attachCsrf = true (default) → cookie + valid CSRF header.
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await cookieClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(MusicSamplesTestConstants.SmallFileSizeBytes),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-AUTH-1 — no credentials → 401
    // =========================================================================

    [Fact]
    public async Task Upload_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-2 — malformed Bearer token → 401
    // =========================================================================

    [Fact]
    public async Task Upload_MalformedBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("this.is.not.a.valid.jwt");

        var response = await HttpClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-3 — expired JWT → 401
    // =========================================================================

    [Fact]
    public async Task Upload_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var expiredToken = await Factory.MintExpiredTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(expiredToken);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4a — JWT signed with wrong key → 401
    // =========================================================================

    [Fact]
    public async Task Upload_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4b — JWT with wrong issuer → 401
    // =========================================================================

    [Fact]
    public async Task Upload_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-4c — JWT with wrong audience → 401
    // =========================================================================

    [Fact]
    public async Task Upload_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email);
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-5 — invalid/expired cookie → 401
    // =========================================================================

    [Fact]
    public async Task Upload_InvalidAuthCookie_Returns401()
    {
        var client = Factory.CreateApiClient();
        // Inject a bogus auth cookie value without ever performing a real login.
        client.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=totally-invalid-cookie-value");

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-AUTH-6 — both JWT and cookie auth modes reach the handler
    // =========================================================================

    [Fact]
    public async Task Upload_JwtAuthMode_ReachesHandler()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var jwtClient = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await jwtClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "JWT bearer auth must reach the handler");
    }

    [Fact]
    public async Task Upload_CookieAuthMode_ReachesHandler()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var cookieClient = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await cookieClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK,
            "Cookie auth must reach the handler when CSRF token is valid");
    }

    // =========================================================================
    // CC-GA-1 — valid token but user row deleted → 401
    // =========================================================================

    [Fact]
    public async Task Upload_TokenForNonexistentUser_Returns401()
    {
        // MintTokenAsync for a random Guid not in the database simulates a deleted user.
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        var client = Factory.CreateApiClient();
        client.SetBearerToken(token);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-2 — unconfirmed email → 401
    // =========================================================================

    [Fact]
    public async Task Upload_UnconfirmedUser_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-3 — deactivated user → 401
    // =========================================================================

    [Fact]
    public async Task Upload_DeactivatedUser_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-4 — first-login user (IsFirstLogin = true) → 401
    //           (endpoint uses checkForFirstLogin: true, the default)
    // =========================================================================

    [Fact]
    public async Task Upload_FirstLoginUser_Returns401()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidMp3(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
