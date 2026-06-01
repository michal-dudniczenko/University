using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Soundmates.IntegrationTests.ProfilePictures;

public sealed class UploadProfilePictureTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static readonly Uri UploadRouteUri = new(ProfilePicturesTestConstants.UploadRoute, UriKind.Relative);

    // =========================================================================
    // Happy paths
    // =========================================================================

    [Fact]
    public async Task Upload_JpegFile_ReturnsOk_AndRowAdded_WithDisplayOrderZero()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pictures = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .Where(pp => pp.UserId == user.Id)
                .ToListAsync());

        pictures.Should().HaveCount(1);
        pictures[0].FileName.Should().EndWith(".jpeg", "extension is lowercased from the uploaded file");
        pictures[0].DisplayOrder.Should().Be(0, "first picture gets DisplayOrder = currentCount (0)");
        pictures[0].UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Upload_JpgFile_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var pictures = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .Where(pp => pp.UserId == user.Id)
                .ToListAsync());

        pictures.Should().HaveCount(1);
        pictures[0].FileName.Should().EndWith(".jpg");
    }

    // =========================================================================
    // Handler validation failures
    // =========================================================================

    [Fact]
    public async Task Upload_DisallowedContentType_AllowedExtension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var content = MultipartContentHelper.BuildFileContent(
            new byte[1024], "picture.jpeg", ProfilePicturesTestConstants.DisallowedContentType);

        var response = await client.PostAsync(UploadRouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_AllowedContentType_DisallowedExtension_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var content = MultipartContentHelper.BuildFileContent(
            new byte[1024],
            "picture" + ProfilePicturesTestConstants.DisallowedExtension,
            MultipartContentHelper.ValidJpegContentType);

        var response = await client.PostAsync(UploadRouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_OversizeFile_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var content = MultipartContentHelper.BuildFileOfSize(
            ProfilePicturesTestConstants.OversizeBytes,
            "picture.jpeg",
            MultipartContentHelper.ValidJpegContentType);

        var response = await client.PostAsync(UploadRouteUri, content, TestContext.Current.CancellationToken);

        // The app's ApplicationConstants size check returns 400 before the framework limit is reached.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_ExactlyMaxSizeFile_ReturnsOk()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var content = MultipartContentHelper.BuildFileOfSize(
            ProfilePicturesTestConstants.ExactMaxSizeBytes,
            "picture.jpeg",
            MultipartContentHelper.ValidJpegContentType);

        var response = await client.PostAsync(UploadRouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Upload_WhenUserAlreadyHasFivePictures_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        for (var i = 0; i < ProfilePicturesTestConstants.MaxPictureCount; i++)
        {
            await Factory.SeedProfilePictureAsync(user.Id, $"pic{i}.jpg", i);
        }

        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_FifthPicture_ReturnsOk_SixthPicture_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();

        for (var i = 0; i < ProfilePicturesTestConstants.MaxPictureCount - 1; i++)
        {
            await Factory.SeedProfilePictureAsync(user.Id, $"pic{i}.jpg", i);
        }

        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var fifthResponse = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);
        fifthResponse.StatusCode.Should().Be(HttpStatusCode.OK, "fifth upload should succeed");

        var pictures = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .Where(pp => pp.UserId == user.Id)
                .OrderBy(pp => pp.DisplayOrder)
                .ToListAsync());

        pictures.Should().HaveCount(5);
        pictures[^1].DisplayOrder.Should().Be(4, "fifth picture gets DisplayOrder = 4");

        var sixthResponse = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);
        sixthResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "sixth upload must be rejected");
    }

    // =========================================================================
    // Edge cases
    // =========================================================================

    [Fact]
    public async Task Upload_MissingFileField_Returns400()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var emptyForm = new MultipartFormDataContent();
        var response = await client.PostAsync(UploadRouteUri, emptyForm, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upload_MultipleUploads_ExtensionLowercased_DisplayOrderIncrements()
    {
        var user = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        await client.PostAsync(UploadRouteUri, MultipartContentHelper.ValidJpeg(), TestContext.Current.CancellationToken);
        await client.PostAsync(UploadRouteUri, MultipartContentHelper.ValidJpg(), TestContext.Current.CancellationToken);
        await client.PostAsync(UploadRouteUri, MultipartContentHelper.ValidJpeg(), TestContext.Current.CancellationToken);

        var pictures = await Factory.ExecuteDbContextAsync(async db =>
            await db.ProfilePictures
                .AsNoTracking()
                .Where(pp => pp.UserId == user.Id)
                .OrderBy(pp => pp.DisplayOrder)
                .ToListAsync());

        pictures.Should().HaveCount(3);

        pictures[0].DisplayOrder.Should().Be(0);
        pictures[1].DisplayOrder.Should().Be(1);
        pictures[2].DisplayOrder.Should().Be(2);

        pictures.Should().AllSatisfy(pp =>
            pp.FileName.Should().MatchRegex(@"\.[a-z]+$", "file extension must be lower-cased"));
    }

    // =========================================================================
    // CC-AUTH-1 — no credentials
    // =========================================================================

    [Fact]
    public async Task Upload_NoCredentials_Returns401()
    {
        var response = await HttpClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =========================================================================
    // CC-GA-1..4 — GetAuthorizedUserAsync gate (checkForFirstLogin: true)
    // =========================================================================

    [Fact]
    public async Task Upload_TokenForDeletedUser_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);

        var response = await HttpClient.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_UnconfirmedEmailUser_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_DeactivatedUser_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_FirstLoginUser_Returns401()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await client.PostAsync(
            UploadRouteUri,
            MultipartContentHelper.ValidJpeg(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
