using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Users.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Soundmates.IntegrationTests.Users;

public sealed class UpdateProfileTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    private static readonly Uri RouteUri = new(UsersTestConstants.UpdateProfileRoute, UriKind.Relative);

    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
        Justification = "Return type must be the polymorphic base so System.Text.Json writes the userType discriminator.")]
    private static UpdateUserProfileRequest ArtistBody(
        string name = "Valid Artist",
        string description = "A description.",
        string? countryId = null,
        string? cityId = null,
        string? genderId = null,
        string birthDate = "1995-06-15",
        IList<string>? tagsIds = null,
        IList<string>? musicSamplesOrder = null,
        IList<string>? profilePicturesOrder = null) =>
        new UpdateUserProfileArtistRequest
        {
            Name = name,
            Description = description,
            CountryId = countryId ?? Guid.NewGuid().ToString(),
            CityId = cityId ?? Guid.NewGuid().ToString(),
            GenderId = genderId ?? Guid.NewGuid().ToString(),
            BirthDate = birthDate,
            TagsIds = tagsIds ?? [],
            MusicSamplesOrder = musicSamplesOrder ?? [],
            ProfilePicturesOrder = profilePicturesOrder ?? []
        };

    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance",
        Justification = "Return type must be the polymorphic base so System.Text.Json writes the userType discriminator.")]
    private static UpdateUserProfileRequest BandBody(
        string name = "Valid Band",
        string description = "A description.",
        string? countryId = null,
        string? cityId = null,
        IList<string>? tagsIds = null,
        IList<BandMemberRequestDto>? bandMembers = null,
        IList<string>? musicSamplesOrder = null,
        IList<string>? profilePicturesOrder = null) =>
        new UpdateUserProfileBandRequest
        {
            Name = name,
            Description = description,
            CountryId = countryId ?? Guid.NewGuid().ToString(),
            CityId = cityId ?? Guid.NewGuid().ToString(),
            TagsIds = tagsIds ?? [],
            BandMembers = bandMembers ?? [],
            MusicSamplesOrder = musicSamplesOrder ?? [],
            ProfilePicturesOrder = profilePicturesOrder ?? []
        };

    private async Task<UpdateUserProfileRequest> ValidArtistBodyAsync(int tagCount = 1)
    {
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(tagCount);
        return ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            tagsIds: tagIds.Select(t => t.ToString()).ToList());
    }

    private static Task<HttpResponseMessage> PutAsync(HttpClient client, UpdateUserProfileRequest body) =>
        client.PutAsJsonAsync(RouteUri, body, TestJson.Options, TestContext.Current.CancellationToken);

    // =======================================================================
    // Happy paths
    // =======================================================================

    [Fact]
    public async Task UpdateProfile_FirstTimeArtist_CreatesArtistRowAndFlipsFlags()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(2);
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "s.mp3", 5);
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "p.jpg", 5);

        var body = ArtistBody(
            name: "Jane Doe",
            description: "Guitarist",
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            birthDate: "1992-04-01",
            tagsIds: tagIds.Select(t => t.ToString()).ToList(),
            musicSamplesOrder: [sampleId.ToString()],
            profilePicturesOrder: [pictureId.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await PutAsync(client, body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var dbUser = await db.Users
                .Include(u => u.Tags)
                .Include(u => u.MusicSamples)
                .Include(u => u.ProfilePictures)
                .AsNoTracking()
                .FirstAsync(u => u.Id == user.Id);

            dbUser.IsBand.Should().Be(false);
            dbUser.IsFirstLogin.Should().BeFalse();
            dbUser.Name.Should().Be("Jane Doe");
            dbUser.ProfileDescription.Should().Be("Guitarist");
            dbUser.CountryId.Should().Be(city.CountryId);
            dbUser.CityId.Should().Be(city.Id);
            dbUser.Tags.Select(t => t.Id).Should().BeEquivalentTo(tagIds);
            dbUser.MusicSamples.Single(ms => ms.Id == sampleId).DisplayOrder.Should().Be(0);
            dbUser.ProfilePictures.Single(pp => pp.Id == pictureId).DisplayOrder.Should().Be(0);

            var artist = await db.Artists.AsNoTracking().SingleAsync(a => a.UserId == user.Id);
            artist.BirthDate.Should().Be(new DateOnly(1992, 4, 1));
            artist.GenderId.Should().Be(genderId);
        });
    }

    [Fact]
    public async Task UpdateProfile_FirstTimeBand_CreatesBandRowWithOrderedMembers()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var tagIds = await Factory.GetBandTagIdsAsync(2);

        var body = BandBody(
            name: "The Band",
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            tagsIds: tagIds.Select(t => t.ToString()).ToList(),
            bandMembers:
            [
                new BandMemberRequestDto("Alice", 30, bandRoleId.ToString()),
                new BandMemberRequestDto("Bob", 28, bandRoleId.ToString())
            ]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await PutAsync(client, body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var dbUser = await db.Users.Include(u => u.Tags).AsNoTracking().FirstAsync(u => u.Id == user.Id);
            dbUser.IsBand.Should().Be(true);
            dbUser.IsFirstLogin.Should().BeFalse();
            dbUser.Tags.Select(t => t.Id).Should().BeEquivalentTo(tagIds);

            var band = await db.Bands.Include(b => b.Members).AsNoTracking().SingleAsync(b => b.UserId == user.Id);
            band.Members.OrderBy(m => m.DisplayOrder).Select(m => m.Name)
                .Should().ContainInOrder("Alice", "Bob");
            band.Members.Single(m => m.Name == "Alice").DisplayOrder.Should().Be(0);
            band.Members.Single(m => m.Name == "Bob").DisplayOrder.Should().Be(1);
        });
    }

    [Fact]
    public async Task UpdateProfile_ExistingArtist_UpdatesRowInPlace()
    {
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var user = await Factory.CreateOnboardedArtistAsync(
            countryId: city.CountryId, cityId: city.Id, genderId: genderId,
            birthDate: new DateOnly(1980, 1, 1));

        var newGenderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            name: "Updated Name",
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: newGenderId.ToString(),
            birthDate: "1999-12-31");

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await PutAsync(client, body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var artists = await db.Artists.AsNoTracking().Where(a => a.UserId == user.Id).ToListAsync();
            artists.Should().HaveCount(1, "the existing artist row should be updated in place, not duplicated");
            artists[0].BirthDate.Should().Be(new DateOnly(1999, 12, 31));

            var dbUser = await db.Users.AsNoTracking().FirstAsync(u => u.Id == user.Id);
            dbUser.Name.Should().Be("Updated Name");
        });
    }

    [Fact]
    public async Task UpdateProfile_ExistingBand_ClearsAndReaddsMembersInOrder()
    {
        var city = await Factory.GetAnyCityAsync();
        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var user = await Factory.CreateOnboardedBandAsync(
            countryId: city.CountryId, cityId: city.Id, memberCount: 3, bandRoleId: bandRoleId);

        var body = BandBody(
            name: "Reformed Band",
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            bandMembers:
            [
                new BandMemberRequestDto("NewFirst", 40, bandRoleId.ToString()),
                new BandMemberRequestDto("NewSecond", 41, bandRoleId.ToString())
            ]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        var response = await PutAsync(client, body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var band = await db.Bands.Include(b => b.Members).AsNoTracking().SingleAsync(b => b.UserId == user.Id);
            band.Members.Should().HaveCount(2, "the original 3 members are cleared and replaced");
            band.Members.OrderBy(m => m.DisplayOrder).Select(m => m.Name)
                .Should().ContainInOrder("NewFirst", "NewSecond");
        });
    }

    [Fact]
    public async Task UpdateProfile_FirstCall_FlipsIsFirstLoginAndUnlocksGuardedEndpoints()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var body = await ValidArtistBodyAsync();

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);

        var target = await Factory.CreateOnboardedArtistAsync();
        var followUp = await client.GetAsync(
            new Uri(UsersTestConstants.OtherProfileRoute(target.Id), UriKind.Relative),
            TestContext.Current.CancellationToken);
        followUp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =======================================================================
    // Body binding / discriminator (B1)
    // =======================================================================

    [Fact]
    public async Task UpdateProfile_UnknownUserType_Returns400()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var json = """
            {"userType":"alien","name":"x","description":"x","countryId":"x","cityId":"x",
             "tagsIds":[],"musicSamplesOrder":[],"profilePicturesOrder":[]}
            """;
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync(RouteUri, content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // =======================================================================
    // Artist validation (VA1..VA9) -> 422
    // =======================================================================

    private async Task AssertProfileValidationFailsAsync(UpdateUserProfileRequest body, string expectedKey)
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(user);

        var response = await PutAsync(client, body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(expectedKey);
    }

    [Fact]
    public Task UpdateProfile_ArtistNameEmpty_Returns422() =>
        AssertProfileValidationFailsAsync(ArtistBody(name: ""), UsersTestConstants.NameKey);

    [Fact]
    public Task UpdateProfile_ArtistDescriptionTooLong_Returns422() =>
        AssertProfileValidationFailsAsync(
            ArtistBody(description: new string('a', UsersTestConstants.MaxDescriptionLength + 1)),
            UsersTestConstants.DescriptionKey);

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistCountryIdInvalid_Returns422(string countryId) =>
        AssertProfileValidationFailsAsync(ArtistBody(countryId: countryId), UsersTestConstants.CountryIdKey);

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistCityIdInvalid_Returns422(string cityId) =>
        AssertProfileValidationFailsAsync(ArtistBody(cityId: cityId), UsersTestConstants.CityIdKey);

    [Fact]
    public Task UpdateProfile_ArtistTagsIdsNull_Returns422() =>
        AssertProfileValidationFailsAsync(
            new UpdateUserProfileArtistRequest
            {
                Name = "Valid Artist",
                Description = "A description.",
                CountryId = Guid.NewGuid().ToString(),
                CityId = Guid.NewGuid().ToString(),
                GenderId = Guid.NewGuid().ToString(),
                BirthDate = "1995-06-15",
                TagsIds = null!,
                MusicSamplesOrder = [],
                ProfilePicturesOrder = []
            },
            UsersTestConstants.TagsIdsKey);

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistTagsElementInvalid_Returns422(string element) =>
        AssertProfileValidationFailsAsync(ArtistBody(tagsIds: [element]), UsersTestConstants.TagsIdsKey + "[0]");

    [Fact]
    public Task UpdateProfile_ArtistMusicSamplesOrderNull_Returns422()
    {
        var body = ArtistBody();
        body.MusicSamplesOrder = null!;
        return AssertProfileValidationFailsAsync(body, UsersTestConstants.MusicSamplesOrderKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistMusicSamplesElementInvalid_Returns422(string element) =>
        AssertProfileValidationFailsAsync(
            ArtistBody(musicSamplesOrder: [element]), UsersTestConstants.MusicSamplesOrderKey + "[0]");

    [Fact]
    public Task UpdateProfile_ArtistProfilePicturesOrderNull_Returns422()
    {
        var body = ArtistBody();
        body.ProfilePicturesOrder = null!;
        return AssertProfileValidationFailsAsync(body, UsersTestConstants.ProfilePicturesOrderKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistProfilePicturesElementInvalid_Returns422(string element) =>
        AssertProfileValidationFailsAsync(
            ArtistBody(profilePicturesOrder: [element]), UsersTestConstants.ProfilePicturesOrderKey + "[0]");

    [Theory]
    [InlineData("not-a-date")]
    [InlineData("1899-12-31")]
    [InlineData("2999-01-01")]
    public Task UpdateProfile_ArtistBirthDateInvalid_Returns422(string birthDate) =>
        AssertProfileValidationFailsAsync(ArtistBody(birthDate: birthDate), UsersTestConstants.BirthDateKey);

    [Fact]
    public async Task UpdateProfile_ArtistBirthDateMinBoundary_Passes()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            birthDate: UsersTestConstants.MinBirthDate);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_ArtistBirthDateTodayBoundary_Passes()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            birthDate: today);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_ArtistGenderIdInvalid_Returns422(string genderId) =>
        AssertProfileValidationFailsAsync(ArtistBody(genderId: genderId), UsersTestConstants.GenderIdKey);

    // =======================================================================
    // Band validation (VB1, VB8..VB10) -> 422
    // =======================================================================

    private static List<BandMemberRequestDto> ValidMembers(int count = 1) =>
        Enumerable.Range(0, count)
            .Select(i => new BandMemberRequestDto($"Member {i + 1}", 25, Guid.NewGuid().ToString()))
            .ToList();

    [Fact]
    public Task UpdateProfile_BandNameEmpty_Returns422() =>
        AssertProfileValidationFailsAsync(BandBody(name: "", bandMembers: ValidMembers()), UsersTestConstants.NameKey);

    [Fact]
    public Task UpdateProfile_BandMembersNull_Returns422()
    {
        var body = (UpdateUserProfileBandRequest)BandBody();
        body.BandMembers = null!;
        return AssertProfileValidationFailsAsync(body, UsersTestConstants.BandMembersKey);
    }

    [Fact]
    public Task UpdateProfile_BandMembersCountAtMax_Returns422() =>
        AssertProfileValidationFailsAsync(
            BandBody(bandMembers: ValidMembers(UsersTestConstants.MaximumBandMembersCount)), UsersTestConstants.BandMembersKey);

    [Fact]
    public async Task UpdateProfile_BandMembersCount99_Passes()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var members = Enumerable.Range(0, UsersTestConstants.MaximumBandMembersCount - 1)
            .Select(i => new BandMemberRequestDto($"M{i}", 25, bandRoleId.ToString()))
            .ToList();
        var body = BandBody(
            countryId: city.CountryId.ToString(), cityId: city.Id.ToString(), bandMembers: members);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_BandMembersCountZero_Passes()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var body = BandBody(countryId: city.CountryId.ToString(), cityId: city.Id.ToString(), bandMembers: []);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public Task UpdateProfile_BandMemberNameEmpty_Returns422() =>
        AssertProfileValidationFailsAsync(
            BandBody(bandMembers: [new BandMemberRequestDto("", 25, Guid.NewGuid().ToString())]),
            "BandMembers[0].Name");

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public Task UpdateProfile_BandMemberAgeOutOfRange_Returns422(int age) =>
        AssertProfileValidationFailsAsync(
            BandBody(bandMembers: [new BandMemberRequestDto("Member", age, Guid.NewGuid().ToString())]),
            "BandMembers[0].Age");

    [Theory]
    [InlineData("")]
    [InlineData(UsersTestConstants.NotAGuid)]
    public Task UpdateProfile_BandMemberBandRoleIdInvalid_Returns422(string bandRoleId) =>
        AssertProfileValidationFailsAsync(
            BandBody(bandMembers: [new BandMemberRequestDto("Member", 25, bandRoleId)]),
            "BandMembers[0].BandRoleId");

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public async Task UpdateProfile_BandMemberAgeBoundaries_Pass(int age)
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var body = BandBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            bandMembers: [new BandMemberRequestDto("Member", age, bandRoleId.ToString())]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // =======================================================================
    // Handler post-validation failures (throw -> 500)
    // =======================================================================

    private static async Task AssertServerErrorAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var problem = await response.ReadRequiredAsync<TestProblemDetails>();
        problem.TraceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateProfile_NonexistentTagId_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            tagsIds: [UsersTestConstants.NonexistentGuid()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_ArtistWithBandCategoryTag_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var bandTagIds = await Factory.GetBandTagIdsAsync(1);
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            tagsIds: bandTagIds.Select(t => t.ToString()).ToList());

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_BandWithArtistCategoryTag_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var artistTagIds = await Factory.GetArtistTagIdsAsync(1);
        var body = BandBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            tagsIds: artistTagIds.Select(t => t.ToString()).ToList(),
            bandMembers: []);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_DuplicateMusicSampleOrder_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var sampleId = await Factory.SeedMusicSampleAsync(user.Id, "dup.mp3", 0);
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            musicSamplesOrder: [sampleId.ToString(), sampleId.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_NonOwnedMusicSampleOrder_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var otherSampleId = await Factory.SeedMusicSampleAsync(other.Id, "foreign.mp3", 0);
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            musicSamplesOrder: [otherSampleId.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_DuplicateProfilePictureOrder_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var pictureId = await Factory.SeedProfilePictureAsync(user.Id, "dup.jpg", 0);
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            profilePicturesOrder: [pictureId.ToString(), pictureId.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_NonOwnedProfilePictureOrder_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        var otherPictureId = await Factory.SeedProfilePictureAsync(other.Id, "foreign.jpg", 0);
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            profilePicturesOrder: [otherPictureId.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_NonexistentCountryFk_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: UsersTestConstants.NonexistentGuid(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString());

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_NonexistentGenderFk_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: UsersTestConstants.NonexistentGuid());

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    [Fact]
    public async Task UpdateProfile_NonexistentBandRoleFk_Returns500()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var body = BandBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            bandMembers: [new BandMemberRequestDto("Member", 25, UsersTestConstants.NonexistentGuid())]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        await AssertServerErrorAsync(await PutAsync(client, body));
    }

    // =======================================================================
    // Edge (E2..E5)
    // =======================================================================

    [Fact]
    public async Task UpdateProfile_SwitchArtistToBand_LeavesOldArtistRow()
    {
        var city = await Factory.GetAnyCityAsync();
        var user = await Factory.CreateOnboardedArtistAsync(countryId: city.CountryId, cityId: city.Id);

        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var body = BandBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            bandMembers: [new BandMemberRequestDto("Member", 25, bandRoleId.ToString())]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var dbUser = await db.Users.AsNoTracking().FirstAsync(u => u.Id == user.Id);
            dbUser.IsBand.Should().Be(true);

            (await db.Artists.AsNoTracking().AnyAsync(a => a.UserId == user.Id))
                .Should().BeTrue("switching type does not delete the previous Artist row");
            (await db.Bands.AsNoTracking().AnyAsync(b => b.UserId == user.Id))
                .Should().BeTrue("the new Band row should exist");
        });
    }

    [Fact]
    public async Task UpdateProfile_SubsetMediaOrder_DropsOmittedItems()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var a = await Factory.SeedMusicSampleAsync(user.Id, "a.mp3", 0);
        var b = await Factory.SeedMusicSampleAsync(user.Id, "b.mp3", 1);
        var c = await Factory.SeedMusicSampleAsync(user.Id, "c.mp3", 2);

        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            musicSamplesOrder: [a.ToString(), c.ToString()]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var samples = await db.MusicSamples.AsNoTracking().Where(ms => ms.UserId == user.Id).ToListAsync();
            samples.Select(s => s.Id).Should().BeEquivalentTo([a, c]);
            samples.Should().NotContain(s => s.Id == b);
            samples.Single(s => s.Id == a).DisplayOrder.Should().Be(0);
            samples.Single(s => s.Id == c).DisplayOrder.Should().Be(1);
        });
    }

    [Fact]
    public async Task UpdateProfile_NameDescriptionAndMemberName_AreTrimmed()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        var city = await Factory.GetAnyCityAsync();
        var bandRoleId = await Factory.GetAnyBandRoleIdAsync();
        var body = BandBody(
            name: "  Spaced Band  ",
            description: "  spaced desc  ",
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            bandMembers: [new BandMemberRequestDto("  Spaced Member  ", 25, bandRoleId.ToString())]);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var dbUser = await db.Users.AsNoTracking().FirstAsync(u => u.Id == user.Id);
            dbUser.Name.Should().Be("Spaced Band");
            dbUser.ProfileDescription.Should().Be("spaced desc");

            var band = await db.Bands.Include(b => b.Members).AsNoTracking().SingleAsync(b => b.UserId == user.Id);
            band.Members.Single().Name.Should().Be("Spaced Member");
        });
    }

    [Fact]
    public async Task UpdateProfile_EmptyOrderLists_ClearAllMedia()
    {
        var user = await Factory.CreateFirstLoginUserAsync();
        await Factory.SeedMusicSampleAsync(user.Id, "x.mp3", 0);
        await Factory.SeedProfilePictureAsync(user.Id, "x.jpg", 0);

        var city = await Factory.GetAnyCityAsync();
        var genderId = await Factory.GetAnyGenderIdAsync();
        var body = ArtistBody(
            countryId: city.CountryId.ToString(),
            cityId: city.Id.ToString(),
            genderId: genderId.ToString(),
            musicSamplesOrder: [],
            profilePicturesOrder: []);

        var client = await Factory.CreateAuthenticatedClientAsync(user);
        (await PutAsync(client, body)).StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            (await db.MusicSamples.AsNoTracking().AnyAsync(ms => ms.UserId == user.Id)).Should().BeFalse();
            (await db.ProfilePictures.AsNoTracking().AnyAsync(pp => pp.UserId == user.Id)).Should().BeFalse();
        });
    }

    // =======================================================================
    // CC-AUTH (framework auth layer)
    // =======================================================================

    [Fact]
    public async Task UpdateProfile_NoCredentials_Returns401()
    {
        var response = await PutAsync(HttpClient, ArtistBody());
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // =======================================================================
    // CC-GA (GetAuthorizedUserAsync gate; checkForFirstLogin:false)
    // =======================================================================

    [Fact]
    public async Task UpdateProfile_DeletedCaller_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        (await PutAsync(HttpClient, ArtistBody())).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_UnconfirmedCaller_Returns401()
    {
        var user = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(user.Id, user.Email));
        (await PutAsync(HttpClient, ArtistBody())).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_DeactivatedCaller_Returns401()
    {
        var user = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(user.Id, user.Email));
        (await PutAsync(HttpClient, ArtistBody())).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
