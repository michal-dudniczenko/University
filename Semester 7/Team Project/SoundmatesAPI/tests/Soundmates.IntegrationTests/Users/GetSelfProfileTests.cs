using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Users.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Users;

public sealed class GetSelfProfileTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(UsersTestConstants.SelfProfileRoute, UriKind.Relative);

    [Fact]
    public async Task GetSelfProfile_OnboardedArtist_ReturnsFullProfileWithEmailAndOrderedMedia()
    {
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(2);
        var birthDate = new DateOnly(1995, 6, 15);

        var user = await Factory.CreateOnboardedArtistAsync(
            countryId: city.CountryId, cityId: city.Id, genderId: genderId,
            birthDate: birthDate, profileTagIds: tagIds);

        await Factory.SeedMusicSampleAsync(user.Id, "second.mp3", displayOrder: 1);
        await Factory.SeedMusicSampleAsync(user.Id, "first.mp3", displayOrder: 0);
        await Factory.SeedProfilePictureAsync(user.Id, "pic-b.jpg", displayOrder: 1);
        await Factory.SeedProfilePictureAsync(user.Id, "pic-a.jpg", displayOrder: 0);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<SelfUserProfileArtistResponse>();

        body.Id.Should().Be(user.Id);
        body.IsBand.Should().Be(false);
        body.IsFirstLogin.Should().BeFalse();
        body.Email.Should().Be(user.Email);
        body.BirthDate.Should().Be(birthDate);
        body.GenderId.Should().Be(genderId);
        body.CountryId.Should().Be(city.CountryId);
        body.CityId.Should().Be(city.Id);
        body.TagsIds.Should().BeEquivalentTo(tagIds);

        var sampleUrls = body.MusicSamples.Select(ms => ms.FileUrl).ToList();
        sampleUrls.Should().HaveCount(2);
        sampleUrls[0].Should().Contain("first.mp3").And.StartWith("http");
        sampleUrls[1].Should().Contain("second.mp3").And.StartWith("http");

        var pictureUrls = body.ProfilePictures.Select(pp => pp.FileUrl).ToList();
        pictureUrls.Should().HaveCount(2);
        pictureUrls[0].Should().Contain("pic-a.jpg").And.StartWith("http");
        pictureUrls[1].Should().Contain("pic-b.jpg").And.StartWith("http");
    }

    [Fact]
    public async Task GetSelfProfile_OnboardedBand_ReturnsBandMembersWithEmail()
    {
        var user = await Factory.CreateOnboardedBandAsync(memberCount: 3);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<SelfUserProfileBandResponse>();

        body.Id.Should().Be(user.Id);
        body.IsBand.Should().Be(true);
        body.Email.Should().Be(user.Email);
        body.BandMembers.Should().HaveCount(3);
        body.BandMembers.Select(m => m.Name).Should().ContainInOrder("Member 1", "Member 2", "Member 3");
    }

    [Fact]
    public async Task GetSelfProfile_FirstLoginUser_ReturnsBaseProfileWithNullsAndEmptyCollections()
    {
        // checkForFirstLogin:false, so a first-login user is allowed through.
        var user = await Factory.CreateFirstLoginUserAsync();

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<GetSelfUserProfileResponse>();

        body.Id.Should().Be(user.Id);
        body.IsBand.Should().BeNull();
        body.IsFirstLogin.Should().BeTrue();
        body.Email.Should().Be(user.Email);
        body.CountryId.Should().BeNull();
        body.CityId.Should().BeNull();
        body.TagsIds.Should().BeEmpty();
        body.MusicSamples.Should().BeEmpty();
        body.ProfilePictures.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSelfProfile_IsBandTrueButNoBandRow_Returns404()
    {
        var user = await Factory.CreateUserAsync(new SeedUserOptions
        {
            IsBand = true,
            IsFirstLogin = false,
            Name = "Orphan Band"
        });

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSelfProfile_IsBandFalseButNoArtistRow_Returns404()
    {
        var user = await Factory.CreateUserAsync(new SeedUserOptions
        {
            IsBand = false,
            IsFirstLogin = false,
            Name = "Orphan Artist"
        });

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await client.GetAsync(RouteUri, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSelfProfile_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSelfProfile_DeletedCaller_Returns401()
    {
        var token = await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local");
        HttpClient.SetBearerToken(token);
        (await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSelfProfile_UnconfirmedCaller_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        var token = await Factory.MintTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);
        (await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSelfProfile_DeactivatedCaller_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        var token = await Factory.MintTokenAsync(user.Id, user.Email);
        HttpClient.SetBearerToken(token);
        (await HttpClient.GetAsync(RouteUri, TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
