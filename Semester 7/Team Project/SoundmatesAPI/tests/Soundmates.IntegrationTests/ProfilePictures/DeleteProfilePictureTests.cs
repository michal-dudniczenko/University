using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.ProfilePictures;

/// <summary>
/// Integration tests for DELETE /profile-pictures/{profilePictureId} (3.36 in tests-plan.md).
/// Endpoint: authenticated, CSRF filter, GetAuthorizedUserAsync(checkForFirstLogin: true),
/// route GUID validated via GuidValidator.
/// Cross-cutting: CC-AUTH-1..6, CC-GA-1..4, CC-CSRF-1..4, CC-ERR-1.
/// </summary>
public sealed class DeleteProfilePictureTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri DeleteRoute(object pictureId) =>
        new($"{ProfilePicturesTestConstants.DeleteRouteBase}/{pictureId}", UriKind.Relative);

    // =========================================================================
    // Happy paths
    // =========================================================================

    /// <summary>H1 — picture belongs to caller → 200; DB row removed.</summary>
    [Fact]
    public async Task Delete_OwnPicture_ReturnsOk_AndRowRemoved()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "mypic.jpg", 0);

        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .AnyAsync(pp => pp.Id == pictureId));

        exists.Should().BeFalse("the DB row must be deleted after a successful delete call");
    }

    // =========================================================================
    // Validation failures
    // =========================================================================

    /// <summary>V1 — non-GUID route param → 422 with key "fieldName".</summary>
    [Fact]
    public async Task Delete_NonGuidRoute_Returns422()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute("not-a-guid"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var problem = await response.ReadRequiredAsync<Microsoft.AspNetCore.Mvc.ValidationProblemDetails>();
        problem.Errors.Should().ContainKey(ProfilePicturesTestConstants.RouteGuidErrorKey,
            "GuidValidator always uses the literal key 'fieldName'");
    }

    // =========================================================================
    // Handler failures
    // =========================================================================

    /// <summary>F1 — picture not found (well-formed GUID, no row) → 404.</summary>
    [Fact]
    public async Task Delete_NonExistentPicture_Returns404()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>F2 — picture belongs to another user → 401.</summary>
    [Fact]
    public async Task Delete_PictureBelongingToAnotherUser_Returns401()
    {
        var owner = await Factory.CreateOnboardedArtistAsync();
        var caller = await Factory.CreateOnboardedArtistAsync();

        // Seed a picture owned by `owner`.
        var pictureId = await Factory.SeedProfilePictureAsync(owner.Id, "owner-pic.jpg", 0);

        // Authenticate as `caller` (not the owner) and attempt to delete.
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // Edge cases
    // =========================================================================

    /// <summary>E1 — DB row exists but the file is missing on disk → skips file delete,
    /// still removes the row → 200. This is the natural state for seeded pictures because
    /// SeedProfilePictureAsync creates only the DB row, not a real file.</summary>
    [Fact]
    public async Task Delete_FileAlreadyMissingOnDisk_StillRemovesRow_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        // SeedProfilePictureAsync creates only the DB row; no real file exists on disk.
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "no-file-on-disk.jpg", 0);

        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        // Handler checks File.Exists before calling File.Delete; missing file is not an error.
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var exists = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .AnyAsync(pp => pp.Id == pictureId));

        exists.Should().BeFalse("DB row must be removed even when the file was already gone");
    }

    // =========================================================================
    // CSRF — CC-CSRF-1..4
    // =========================================================================

    /// <summary>CC-CSRF-1 — cookie-authenticated DELETE without X-CSRF-TOKEN → 400.</summary>
    [Fact]
    public async Task Delete_CookieAuthenticated_WithoutCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "csrf-test.jpg", 0);

        // attachCsrf: false → valid session but no CSRF header.
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: false);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>CC-CSRF-2 — cookie-authenticated DELETE with invalid CSRF token → 400.</summary>
    [Fact]
    public async Task Delete_CookieAuthenticated_WithInvalidCsrfToken_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "csrf-test2.jpg", 0);

        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bad-csrf-token");

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>CC-CSRF-3 — cookie-authenticated DELETE with valid CSRF token → reaches handler.</summary>
    [Fact]
    public async Task Delete_CookieAuthenticated_WithValidCsrfToken_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "csrf-ok.jpg", 0);

        // attachCsrf: true → the helper fetches and attaches a valid CSRF token.
        var client = await Factory.CreateCookieClientAsync(user, attachCsrf: true);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>CC-CSRF-4 — JWT-authenticated DELETE: CSRF filter is skipped (no token required).</summary>
    [Fact]
    public async Task Delete_JwtAuthenticated_CsrfSkipped_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "jwt-delete.jpg", 0);

        // JWT client does not set the CSRF header.
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-AUTH-1..6 — Authentication layer
    // =========================================================================

    /// <summary>CC-AUTH-1 — no credentials → 401.</summary>
    [Fact]
    public async Task Delete_NoCredentials_Returns401()
    {
        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-2 — malformed Bearer token → 401.</summary>
    [Fact]
    public async Task Delete_MalformedBearerToken_Returns401()
    {
        HttpClient.SetBearerToken("not.a.valid.jwt");

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-3 — expired JWT → 401.</summary>
    [Fact]
    public async Task Delete_ExpiredJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintExpiredTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-4a — JWT signed with wrong key → 401.</summary>
    [Fact]
    public async Task Delete_WrongKeyJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongKeyTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-4b — JWT with wrong issuer → 401.</summary>
    [Fact]
    public async Task Delete_WrongIssuerJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongIssuerTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-4c — JWT with wrong audience → 401.</summary>
    [Fact]
    public async Task Delete_WrongAudienceJwt_Returns401()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var token = await Factory.MintWrongAudienceTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-5 — invalid auth cookie → 401.</summary>
    [Fact]
    public async Task Delete_InvalidAuthCookie_Returns401()
    {
        var client = Factory.CreateApiClient();
        client.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid-cookie-value");
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "any-value");

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-AUTH-6 — valid JWT bearer reaches the handler.</summary>
    [Fact]
    public async Task Delete_ValidJwtBearer_ReachesHandler()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "auth6.jpg", 0);
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(pictureId), TestContext.Current.CancellationToken);

        // Handler is reached; picture owned by caller → 200.
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =========================================================================
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true)
    // =========================================================================

    /// <summary>CC-GA-1 — valid token but user no longer exists in DB → 401.</summary>
    [Fact]
    public async Task Delete_TokenForDeletedUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-GA-2 — EmailConfirmed = false → 401.</summary>
    [Fact]
    public async Task Delete_UnconfirmedEmailUser_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-GA-3 — IsActive = false → 401.</summary>
    [Fact]
    public async Task Delete_DeactivatedUser_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    /// <summary>CC-GA-4 — IsFirstLogin = true → 401 (checkForFirstLogin: true).</summary>
    [Fact]
    public async Task Delete_FirstLoginUser_Returns401()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.DeleteAsync(DeleteRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
