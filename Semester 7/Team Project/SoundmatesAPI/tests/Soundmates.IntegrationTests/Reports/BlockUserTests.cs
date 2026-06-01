using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Soundmates.IntegrationTests.Reports;

public sealed class BlockUserTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static Uri BlockRoute(object userId) =>
        new(ReportsTestConstants.BlockUserRoute(userId), UriKind.Relative);

    [Fact]
    public async Task BlockUser_AdminBlocksActiveUser_Returns204AndDeactivates()
    {
        var admin = await Factory.CreateAdminUserAsync();
        var target = await Factory.CreateOnboardedArtistAsync();

        await Factory.SeedRefreshTokenAsync(target.Id);
        await Factory.SeedRefreshTokenAsync(target.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(admin);
        var response = await client.PostAsync(BlockRoute(target.Id), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var user = await db.Users.AsNoTracking().SingleAsync(u => u.Id == target.Id);
            user.IsActive.Should().BeFalse("target should be deactivated after block");
            user.DeactivatedAt.Should().NotBeNull("DeactivatedAt must be set on block");
            user.DeactivatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            var remainingTokens = await db.RefreshTokens.AsNoTracking()
                .Where(rt => rt.UserId == target.Id)
                .ToListAsync();
            remainingTokens.Should().BeEmpty("all target refresh tokens must be revoked on block");
        });
    }

    [Fact]
    public async Task BlockUser_NoCredentials_Returns401()
    {
        var target = await Factory.CreateOnboardedArtistAsync();

        var response = await HttpClient.PostAsync(BlockRoute(target.Id), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BlockUser_NonAdminCaller_Returns403()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsync(BlockRoute(target.Id), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BlockUser_NonGuidRouteSegment_Returns404()
    {
        var admin = await Factory.CreateAdminUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(admin);

        var response = await client.PostAsync(BlockRoute("not-a-guid"), null, TestContext.Current.CancellationToken);

        // NOTE: 404 is returned here despite the invalid GUID, not 422 as might be expected.
        // Guid.TryParse fails silently and the handler immediately returns a 404 Problem.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BlockUser_NonexistentUser_Returns404()
    {
        var admin = await Factory.CreateAdminUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(admin);
        var phantomId = Guid.NewGuid();

        var response = await client.PostAsync(BlockRoute(phantomId), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BlockUser_AlreadyInactiveTarget_Returns409()
    {
        var admin = await Factory.CreateAdminUserAsync();
        var target = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(admin);

        var response = await client.PostAsync(BlockRoute(target.Id), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BlockUser_AdminBlocksOwnAccount_Returns204()
    {
        // NOTE (gap): BlockUser has no guard preventing an admin from blocking their own account.
        var admin = await Factory.CreateAdminUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(admin);

        var response = await client.PostAsync(BlockRoute(admin.Id), null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var adminUser = await db.Users.AsNoTracking().SingleAsync(u => u.Id == admin.Id);
            adminUser.IsActive.Should().BeFalse("the admin's own account was deactivated — self-block gap");
        });
    }

    [Fact]
    public async Task BlockUser_ThenTargetLogin_Returns403()
    {
        var admin = await Factory.CreateAdminUserAsync();
        var target = await Factory.CreateOnboardedArtistAsync();
        var adminClient = await Factory.CreateAuthenticatedClientAsync(admin);

        var blockResponse = await adminClient.PostAsync(BlockRoute(target.Id), null, TestContext.Current.CancellationToken);
        blockResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var loginResponse = await HttpClient.PostAsJsonAsync(
            new Uri("/auth/login?useCookies=false", UriKind.Relative),
            new LoginRequest(target.Email, target.Password),
            TestJson.Options,
            TestContext.Current.CancellationToken);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden,
            "a deactivated user's login must return 403 per LogIn handler F2");
    }
}
