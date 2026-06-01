using Soundmates.IntegrationTests.Users.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Users;

public sealed class GetOtherUserProfileTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static Uri OtherProfileRoute(string userId) =>
        new(UsersTestConstants.OtherProfileRoute(userId), UriKind.Relative);

    private static Uri OtherProfileRoute(Guid userId) =>
        new(UsersTestConstants.OtherProfileRoute(userId), UriKind.Relative);

    [Fact]
    public async Task GetOtherUserProfile_ArtistTarget_ReturnsProfileWithoutEmail()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(2);
        var birthDate = new DateOnly(1990, 3, 10);

        var target = await Factory.CreateOnboardedArtistAsync(
            countryId: city.CountryId, cityId: city.Id, genderId: genderId,
            birthDate: birthDate, profileTagIds: tagIds);
        await Factory.SeedMusicSampleAsync(target.Id, "later.mp3", 1);
        await Factory.SeedMusicSampleAsync(target.Id, "earlier.mp3", 0);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<OtherUserProfileArtistResponse>();

        body.Id.Should().Be(target.Id);
        body.IsBand.Should().Be(false);
        body.BirthDate.Should().Be(birthDate);
        body.CountryId.Should().Be(city.CountryId);
        body.CityId.Should().Be(city.Id);
        body.TagsIds.Should().BeEquivalentTo(tagIds);
        body.MusicSamples.Select(ms => ms.FileUrl).Should()
            .ContainInOrder(
                $"{TestConstants.ClientBaseAddress}/samples/earlier.mp3",
                $"{TestConstants.ClientBaseAddress}/samples/later.mp3");

        var raw = await response.ReadStringAsync();
        raw.Should().NotContain(target.Email, "other-user profile must not expose Email");
    }

    [Fact]
    public async Task GetOtherUserProfile_BandTarget_ReturnsBandMembers()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateOnboardedBandAsync(memberCount: 2);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<OtherUserProfileBandResponse>();
        body.Id.Should().Be(target.Id);
        body.IsBand.Should().Be(true);
        body.BandMembers.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOtherUserProfile_NonGuidUserId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(UsersTestConstants.NotAGuid), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(UsersTestConstants.RouteGuidErrorKey);
    }

    [Fact]
    public async Task GetOtherUserProfile_TargetNotFound_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(Guid.NewGuid()), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_InactiveTarget_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateDeactivatedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_FirstLoginTarget_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_UnconfirmedTarget_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateUnconfirmedUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_IsBandNullTarget_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateUserAsync(new SeedUserOptions
        {
            IsFirstLogin = false,
            IsBand = null
        });
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_CallerRequestsOwnId_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(caller.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<OtherUserProfileArtistResponse>();
        body.Id.Should().Be(caller.Id);
    }

    [Fact]
    public async Task GetOtherUserProfile_FirstLoginCallerRequestsOwnId_Returns401()
    {
        // GetOtherUserProfile uses checkForFirstLogin:true, so a first-login caller is rejected at
        // the auth gate before the target lookup -> 401.
        var caller = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(caller.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOtherUserProfile_IsBandTrueButNoBandRow_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateUserAsync(new SeedUserOptions
        {
            IsBand = true,
            IsFirstLogin = false,
            Name = "No Band Row"
        });
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_IsBandFalseButNoArtistRow_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var target = await Factory.CreateUserAsync(new SeedUserOptions
        {
            IsBand = false,
            IsFirstLogin = false,
            Name = "No Artist Row"
        });
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOtherUserProfile_NoCredentials_Returns401()
    {
        var target = await Factory.CreateOnboardedArtistAsync();
        var response = await HttpClient.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOtherUserProfile_DeletedCaller_Returns401()
    {
        var target = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        (await HttpClient.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOtherUserProfile_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        var target = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOtherUserProfile_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        var target = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOtherUserProfile_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        var target = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(OtherProfileRoute(target.Id), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
