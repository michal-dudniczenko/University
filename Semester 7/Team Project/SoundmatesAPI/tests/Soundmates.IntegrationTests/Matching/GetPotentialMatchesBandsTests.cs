using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for GET /matching/bands?limit&amp;offset — GetPotentialMatchesBands (3.25 in tests-plan.md).
/// Authenticated · GetAuthorizedUserAsync(true) · pagination (maxLimit 50).
/// </summary>
public sealed class GetPotentialMatchesBandsTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri Route(int limit = 50, int offset = 0) =>
        new($"{MatchingTestConstants.BandsRoute}?limit={limit}&offset={offset}", UriKind.Relative);

    /// <summary>Haversine distance in km, mirroring the inlined endpoint formula (EarthRadius = 6371).</summary>
    private static double HaversineKm(SeededCity a, SeededCity b)
    {
        const double earthRadiusKm = 6371.0;
        var lat1 = double.DegreesToRadians(a.Latitude);
        var lat2 = double.DegreesToRadians(b.Latitude);
        var dLat = lat2 - lat1;
        var dLon = double.DegreesToRadians(b.Longitude) - double.DegreesToRadians(a.Longitude);
        var h = Math.Pow(Math.Sin(dLat / 2.0), 2.0)
            + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2.0), 2.0);
        return 2.0 * earthRadiusKm * Math.Asin(Math.Sqrt(h));
    }

    /// <summary>Directly set the caller's match preference filter columns.</summary>
    private Task SetPreferenceAsync(
        Guid userId,
        bool showBands = true,
        int? maxDistance = null,
        Guid? countryId = null,
        Guid? cityId = null,
        int? minMembers = null,
        int? maxMembers = null,
        IReadOnlyList<Guid>? tagIds = null) =>
        Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.Include(p => p.Tags).FirstAsync(p => p.UserId == userId);
            pref.ShowBands = showBands;
            pref.MaxDistance = maxDistance;
            pref.CountryId = countryId;
            pref.CityId = cityId;
            pref.BandMinMembersCount = minMembers;
            pref.BandMaxMembersCount = maxMembers;
            pref.Tags.Clear();
            if (tagIds is { Count: > 0 })
            {
                var tags = await db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                foreach (var tag in tags) pref.Tags.Add(tag);
            }

            await db.SaveChangesAsync();
        });

    // The /matching/bands endpoint returns TypedResults.Ok(List<OtherUserProfileBandResponse>) —
    // a concrete element type, so System.Text.Json does NOT write the "userType" discriminator
    // (unlike /matching/matches, whose element type is the abstract base). Deserialize into the
    // concrete band type directly; deserializing into the polymorphic base throws because the
    // payload carries no discriminator.
    private async Task<List<OtherUserProfileBandResponse>> GetAsync(TestUser caller, int limit = 50, int offset = 0)
    {
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(limit, offset), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.ReadRequiredAsync<List<OtherUserProfileBandResponse>>();
    }

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — default prefs → eligible bands excluding self & liked/disliked; each includes BandMembers.
    [Fact]
    public async Task GetBands_DefaultPrefs_ReturnsEligibleBands()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var visible = await Factory.CreateOnboardedBandAsync();
        var liked = await Factory.CreateOnboardedBandAsync();
        var disliked = await Factory.CreateOnboardedBandAsync();
        var artist = await Factory.CreateOnboardedArtistAsync();

        await Factory.SeedLikeAsync(caller.Id, liked.Id);
        await Factory.SeedDislikeAsync(caller.Id, disliked.Id);

        var result = await GetAsync(caller);
        var ids = result.Select(r => r.Id).ToList();

        ids.Should().Contain(visible.Id);
        ids.Should().NotContain(liked.Id);
        ids.Should().NotContain(disliked.Id);
        ids.Should().NotContain(artist.Id, "the bands endpoint returns only bands");
        result.Should().AllBeOfType<OtherUserProfileBandResponse>();
        result.Single(r => r.Id == visible.Id).BandMembers.Should().NotBeEmpty();
    }

    // H2 — ShowBands == false → empty list.
    [Fact]
    public async Task GetBands_ShowBandsFalse_ReturnsEmpty()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        await Factory.CreateOnboardedBandAsync();
        await SetPreferenceAsync(caller.Id, showBands: false);

        (await GetAsync(caller)).Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // Filters
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_CountryFilter_OnlyThatCountry()
    {
        var cityA = await Factory.GetAnyCityAsync();
        var otherCountryId = await Factory.ExecuteDbContextAsync(db =>
            db.Countries.AsNoTracking().Where(c => c.Id != cityA.CountryId).Select(c => c.Id).FirstAsync());

        var caller = await Factory.CreateOnboardedArtistAsync(countryId: cityA.CountryId, cityId: cityA.Id);
        var inCountry = await Factory.CreateOnboardedBandAsync(countryId: cityA.CountryId, cityId: cityA.Id);
        var outOfCountry = await Factory.CreateOnboardedBandAsync(countryId: otherCountryId);

        await SetPreferenceAsync(caller.Id, countryId: cityA.CountryId);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(inCountry.Id);
        ids.Should().NotContain(outOfCountry.Id);
    }

    [Fact]
    public async Task GetBands_CityFilter_OnlyThatCity()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var inCity = await Factory.CreateOnboardedBandAsync(cityId: cityA.Id);
        var otherCity = await Factory.CreateOnboardedBandAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, cityId: cityA.Id);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(inCity.Id);
        ids.Should().NotContain(otherCity.Id);
    }

    // F-minmembers — Members.Count >= min; boundary at min inclusive.
    [Fact]
    public async Task GetBands_MinMembersFilter_BoundaryInclusive()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var exactlyThree = await Factory.CreateOnboardedBandAsync(memberCount: 3);
        var twoMembers = await Factory.CreateOnboardedBandAsync(memberCount: 2);

        await SetPreferenceAsync(caller.Id, minMembers: 3);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(exactlyThree.Id);
        ids.Should().NotContain(twoMembers.Id);
    }

    // F-maxmembers — Members.Count <= max; boundary at max inclusive.
    [Fact]
    public async Task GetBands_MaxMembersFilter_BoundaryInclusive()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var exactlyThree = await Factory.CreateOnboardedBandAsync(memberCount: 3);
        var fourMembers = await Factory.CreateOnboardedBandAsync(memberCount: 4);

        await SetPreferenceAsync(caller.Id, maxMembers: 3);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(exactlyThree.Id);
        ids.Should().NotContain(fourMembers.Id);
    }

    // F-tags — band-category overlap filter + ranking; zero-overlap candidates excluded.
    [Fact]
    public async Task GetBands_TagFilter_ExcludesZeroOverlapAndRanksByCount()
    {
        var tagIds = await Factory.GetBandTagIdsAsync(2);

        var caller = await Factory.CreateOnboardedArtistAsync();
        var twoOverlap = await Factory.CreateOnboardedBandAsync(profileTagIds: tagIds);
        var oneOverlap = await Factory.CreateOnboardedBandAsync(profileTagIds: [tagIds[0]]);
        var zeroOverlap = await Factory.CreateOnboardedBandAsync();

        await SetPreferenceAsync(caller.Id, tagIds: tagIds);

        var result = await GetAsync(caller);
        var ids = result.Select(r => r.Id).ToList();

        ids.Should().NotContain(zeroOverlap.Id);
        ids.Should().Contain(twoOverlap.Id);
        ids.Should().Contain(oneOverlap.Id);
        ids.IndexOf(twoOverlap.Id).Should().BeLessThan(ids.IndexOf(oneOverlap.Id));
    }

    // -------------------------------------------------------------------------
    // Distance (Haversine) — D1..D4
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_DistanceFilter_WithinRadius()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];
        var distanceAb = HaversineKm(cityA, cityB);

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var near = await Factory.CreateOnboardedBandAsync(cityId: cityA.Id);
        var far = await Factory.CreateOnboardedBandAsync(cityId: cityB.Id);

        var maxDistance = (int)Math.Floor(distanceAb) - 1;
        maxDistance.Should().BeGreaterThan(0, "the two seeded cities must be more than 1km apart");

        // Origin is the caller's own City; do NOT set the preference CityId (that is a candidate filter).
        await SetPreferenceAsync(caller.Id, maxDistance: maxDistance);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(near.Id);
        ids.Should().NotContain(far.Id);
    }

    [Fact]
    public async Task GetBands_MaxDistanceZero_OnlySameCoordinates()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var sameCoords = await Factory.CreateOnboardedBandAsync(cityId: cityA.Id);
        var otherCoords = await Factory.CreateOnboardedBandAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, maxDistance: 0);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(sameCoords.Id);
        ids.Should().NotContain(otherCoords.Id);
    }

    [Fact]
    public async Task GetBands_CandidateNoCity_ExcludedWhenDistanceFilterActive()
    {
        var cityA = await Factory.GetAnyCityAsync();

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        // CreateOnboardedBandAsync resolves a null city to a real seeded one, so clear it explicitly:
        // a candidate without a city has no computable distance and must be filtered out.
        var noCity = await Factory.CreateOnboardedBandAsync();
        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == noCity.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.CityId, (Guid?)null)));

        await SetPreferenceAsync(caller.Id, maxDistance: 100);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().NotContain(noCity.Id);
    }

    [Fact]
    public async Task GetBands_CallerNoCity_DistanceFilterBypassed()
    {
        var cityB = await Factory.GetAnyCityAsync();

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: null);
        var candidate = await Factory.CreateOnboardedBandAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, maxDistance: 1, cityId: null);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(candidate.Id);
    }

    // -------------------------------------------------------------------------
    // Edge
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_NoPreferenceRow_Returns500()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        await Factory.ExecuteDbContextAsync(async db =>
            await db.UserMatchPreferences.Where(p => p.UserId == caller.Id).ExecuteDeleteAsync());

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetBands_EmptyPool_ReturnsEmpty()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        (await GetAsync(caller)).Should().BeEmpty();
    }

    // E3 — combined filters (country + members + tags) narrow correctly.
    [Fact]
    public async Task GetBands_CombinedFilters_NarrowCorrectly()
    {
        var cityA = await Factory.GetAnyCityAsync();
        var tagIds = await Factory.GetBandTagIdsAsync(1);

        var caller = await Factory.CreateOnboardedArtistAsync(countryId: cityA.CountryId, cityId: cityA.Id);

        var match = await Factory.CreateOnboardedBandAsync(
            countryId: cityA.CountryId, cityId: cityA.Id, memberCount: 4, profileTagIds: tagIds);

        // Fails the member-count filter only.
        var tooSmall = await Factory.CreateOnboardedBandAsync(
            countryId: cityA.CountryId, cityId: cityA.Id, memberCount: 1, profileTagIds: tagIds);

        await SetPreferenceAsync(caller.Id,
            countryId: cityA.CountryId, minMembers: 3, maxMembers: 6, tagIds: tagIds);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(match.Id);
        ids.Should().NotContain(tooSmall.Id);
    }

    // -------------------------------------------------------------------------
    // CC-PAG
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_LimitZero_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 0), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetBands_LimitTooLarge_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 51), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetBands_NegativeOffset_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(offset: -1), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetBands_BothInvalid_Returns422WithBothKeys()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route(limit: 0, offset: -1), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(MatchingTestConstants.LimitErrorKey);
        problem.Errors.Should().ContainKey(MatchingTestConstants.OffsetErrorKey);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(50, 0)]
    public async Task GetBands_ValidBoundaries_Returns200(int limit, int offset)
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit, offset), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBands_NonIntegerLimit_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(new Uri($"{MatchingTestConstants.BandsRoute}?limit=abc&offset=0", UriKind.Relative), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBands_Paging_StableOrderingNoSkipOrRepeat()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var created = new List<Guid>();
        for (var i = 0; i < 3; i++)
        {
            created.Add((await Factory.CreateOnboardedBandAsync()).Id);
        }

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var page1 = await (await client.GetAsync(Route(limit: 2, offset: 0), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<OtherUserProfileBandResponse>>();
        var page2 = await (await client.GetAsync(Route(limit: 2, offset: 2), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<OtherUserProfileBandResponse>>();

        var seen = page1.Concat(page2).Select(p => p.Id).ToList();
        seen.Should().OnlyHaveUniqueItems();
        seen.Should().BeEquivalentTo(created);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_NoCredentials_Returns401()
    {
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_WrongIssuerToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_CookieAuthReachesHandler_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        (await client.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetBands_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBands_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
