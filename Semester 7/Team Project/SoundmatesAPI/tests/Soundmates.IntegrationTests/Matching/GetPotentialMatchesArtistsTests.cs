using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for GET /matching/artists?limit&amp;offset — GetPotentialMatchesArtists (3.24 in tests-plan.md).
/// Authenticated · GetAuthorizedUserAsync(true) · pagination (maxLimit 50).
/// </summary>
public sealed class GetPotentialMatchesArtistsTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri Route(int limit = 50, int offset = 0) =>
        new($"{MatchingTestConstants.ArtistsRoute}?limit={limit}&offset={offset}");

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
        bool showArtists = true,
        int? maxDistance = null,
        Guid? countryId = null,
        Guid? cityId = null,
        int? minAge = null,
        int? maxAge = null,
        Guid? genderId = null,
        IReadOnlyList<Guid>? tagIds = null) =>
        Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.Include(p => p.Tags).FirstAsync(p => p.UserId == userId);
            pref.ShowArtists = showArtists;
            pref.MaxDistance = maxDistance;
            pref.CountryId = countryId;
            pref.CityId = cityId;
            pref.ArtistMinAge = minAge;
            pref.ArtistMaxAge = maxAge;
            pref.ArtistGenderId = genderId;
            pref.Tags.Clear();
            if (tagIds is { Count: > 0 })
            {
                var tags = await db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                foreach (var tag in tags) pref.Tags.Add(tag);
            }

            await db.SaveChangesAsync();
        });

    // The /matching/artists endpoint returns TypedResults.Ok(List<OtherUserProfileArtistResponse>) —
    // a concrete element type, so System.Text.Json does NOT write the "userType" discriminator
    // (unlike /matching/matches, whose element type is the abstract base). Deserialize into the
    // concrete artist type directly; deserializing into the polymorphic base throws because the
    // payload carries no discriminator.
    private async Task<List<OtherUserProfileArtistResponse>> GetAsync(TestUser caller, int limit = 50, int offset = 0)
    {
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(limit, offset), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.ReadRequiredAsync<List<OtherUserProfileArtistResponse>>();
    }

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — default prefs → active+confirmed+non-first-login artists, excluding self & liked/disliked.
    [Fact]
    public async Task GetArtists_DefaultPrefs_ReturnsEligibleArtists()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var visible = await Factory.CreateOnboardedArtistAsync();
        var liked = await Factory.CreateOnboardedArtistAsync();
        var disliked = await Factory.CreateOnboardedArtistAsync();
        var band = await Factory.CreateOnboardedBandAsync();

        await Factory.SeedLikeAsync(caller.Id, liked.Id);
        await Factory.SeedDislikeAsync(caller.Id, disliked.Id);

        var result = await GetAsync(caller);

        var ids = result.Select(r => r.Id).ToList();
        ids.Should().Contain(visible.Id);
        ids.Should().NotContain(caller.Id, "self is excluded");
        ids.Should().NotContain(liked.Id, "already-liked candidates are excluded");
        ids.Should().NotContain(disliked.Id, "already-disliked candidates are excluded");
        ids.Should().NotContain(band.Id, "the artists endpoint returns only artists");
        result.Should().AllBeOfType<OtherUserProfileArtistResponse>();
    }

    // H2 — ShowArtists == false → empty list (short-circuit).
    [Fact]
    public async Task GetArtists_ShowArtistsFalse_ReturnsEmpty()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        await Factory.CreateOnboardedArtistAsync();
        await SetPreferenceAsync(caller.Id, showArtists: false);

        (await GetAsync(caller)).Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // Filters
    // -------------------------------------------------------------------------

    // F-country — only artists in the preferred country.
    [Fact]
    public async Task GetArtists_CountryFilter_OnlyThatCountry()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var otherCountryId = await Factory.ExecuteDbContextAsync(db =>
            db.Countries.AsNoTracking().Where(c => c.Id != cityA.CountryId).Select(c => c.Id).FirstAsync());

        var caller = await Factory.CreateOnboardedArtistAsync(countryId: cityA.CountryId, cityId: cityA.Id);
        var inCountry = await Factory.CreateOnboardedArtistAsync(countryId: cityA.CountryId, cityId: cityA.Id);
        var outOfCountry = await Factory.CreateOnboardedArtistAsync(countryId: otherCountryId);

        await SetPreferenceAsync(caller.Id, countryId: cityA.CountryId);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(inCountry.Id);
        ids.Should().NotContain(outOfCountry.Id);
    }

    // F-city — only artists in the preferred city.
    [Fact]
    public async Task GetArtists_CityFilter_OnlyThatCity()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var inCity = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var otherCity = await Factory.CreateOnboardedArtistAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, cityId: cityA.Id);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(inCity.Id);
        ids.Should().NotContain(otherCity.Id);
    }

    // F-minage — BirthDate <= today.AddYears(-min); a user exactly min years old today is included.
    [Fact]
    public async Task GetArtists_MinAgeFilter_BoundaryInclusive()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);

        // exactly 25 today → included; 24 (one day short) → excluded.
        var exactly25 = await Factory.CreateOnboardedArtistAsync(birthDate: today.AddYears(-25));
        var almost25 = await Factory.CreateOnboardedArtistAsync(birthDate: today.AddYears(-25).AddDays(1));

        await SetPreferenceAsync(caller.Id, minAge: 25);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(exactly25.Id);
        ids.Should().NotContain(almost25.Id);
    }

    // F-maxage — BirthDate > today.AddYears(-(max+1)); max years + 364 days included, max+1 excluded.
    [Fact]
    public async Task GetArtists_MaxAgeFilter_BoundaryInclusive()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);

        // 30 years + 364 days old → included; exactly 31 → excluded.
        var withinMax = await Factory.CreateOnboardedArtistAsync(birthDate: today.AddYears(-31).AddDays(1));
        var tooOld = await Factory.CreateOnboardedArtistAsync(birthDate: today.AddYears(-31));

        await SetPreferenceAsync(caller.Id, maxAge: 30);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(withinMax.Id);
        ids.Should().NotContain(tooOld.Id);
    }

    // F-gender — only the preferred gender.
    [Fact]
    public async Task GetArtists_GenderFilter_OnlyThatGender()
    {
        var genders = await Factory.ExecuteDbContextAsync(db =>
            db.Genders.AsNoTracking().OrderBy(g => g.Name).Select(g => g.Id).Take(2).ToListAsync());
        genders.Should().HaveCountGreaterThanOrEqualTo(2);

        var caller = await Factory.CreateOnboardedArtistAsync();
        var matchingGender = await Factory.CreateOnboardedArtistAsync(genderId: genders[0]);
        var otherGender = await Factory.CreateOnboardedArtistAsync(genderId: genders[1]);

        await SetPreferenceAsync(caller.Id, genderId: genders[0]);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(matchingGender.Id);
        ids.Should().NotContain(otherGender.Id);
    }

    // F-tags — candidates with zero overlapping tags are EXCLUDED; results ranked by overlap count.
    [Fact]
    public async Task GetArtists_TagFilter_ExcludesZeroOverlapAndRanksByCount()
    {
        var tagIds = await Factory.GetArtistTagIdsAsync(2);

        var caller = await Factory.CreateOnboardedArtistAsync();
        var twoOverlap = await Factory.CreateOnboardedArtistAsync(profileTagIds: tagIds);
        var oneOverlap = await Factory.CreateOnboardedArtistAsync(profileTagIds: [tagIds[0]]);
        var zeroOverlap = await Factory.CreateOnboardedArtistAsync();

        await SetPreferenceAsync(caller.Id, tagIds: tagIds);

        var result = await GetAsync(caller);
        var ids = result.Select(r => r.Id).ToList();

        ids.Should().NotContain(zeroOverlap.Id, "zero-overlap candidates are excluded, not just ranked lower");
        ids.Should().Contain(twoOverlap.Id);
        ids.Should().Contain(oneOverlap.Id);
        ids.IndexOf(twoOverlap.Id).Should().BeLessThan(ids.IndexOf(oneOverlap.Id),
            "more tag overlap ranks higher");
    }

    // -------------------------------------------------------------------------
    // Distance (Haversine)
    // -------------------------------------------------------------------------

    // D1 — origin city + MaxDistance > 0 → candidates within MaxDistance survive; nearer ranks higher.
    [Fact]
    public async Task GetArtists_DistanceFilter_WithinRadius()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];
        var distanceAb = HaversineKm(cityA, cityB);

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var near = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);   // distance 0
        var far = await Factory.CreateOnboardedArtistAsync(cityId: cityB.Id);    // distance Ab

        // MaxDistance just below the A↔B distance → only the same-city candidate survives.
        var maxDistance = (int)Math.Floor(distanceAb) - 1;
        maxDistance.Should().BeGreaterThan(0, "the two seeded cities must be more than 1km apart");

        // Origin is the caller's own City; do NOT set the preference CityId (that is a candidate filter).
        await SetPreferenceAsync(caller.Id, maxDistance: maxDistance);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(near.Id);
        ids.Should().NotContain(far.Id);
    }

    // D2 — MaxDistance == 0 + a city → distance filter applies (<= 0), only exact-coordinate candidates survive.
    [Fact]
    public async Task GetArtists_MaxDistanceZero_OnlySameCoordinates()
    {
        var cities = await Factory.GetCitiesAsync(2);
        var cityA = cities[0];
        var cityB = cities[1];

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var sameCoords = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        var otherCoords = await Factory.CreateOnboardedArtistAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, maxDistance: 0);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(sameCoords.Id);
        ids.Should().NotContain(otherCoords.Id);
    }

    // D3 — candidate with City == null → Distance null → excluded when distance filter is active.
    [Fact]
    public async Task GetArtists_CandidateNoCity_ExcludedWhenDistanceFilterActive()
    {
        var cityA = await Factory.GetAnyCityAsync();

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: cityA.Id);
        // CreateOnboardedArtistAsync resolves a null city to a real seeded one, so clear it explicitly:
        // a candidate without a city has no computable distance and must be filtered out.
        var noCity = await Factory.CreateOnboardedArtistAsync();
        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == noCity.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.CityId, (Guid?)null)));

        await SetPreferenceAsync(caller.Id, maxDistance: 100);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().NotContain(noCity.Id);
    }

    // D4 — caller has no city but MaxDistance set → distance filter/scoring ignored; eligible candidates appear.
    [Fact]
    public async Task GetArtists_CallerNoCity_DistanceFilterBypassed()
    {
        var cityB = await Factory.GetAnyCityAsync();

        var caller = await Factory.CreateOnboardedArtistAsync(cityId: null);
        var candidate = await Factory.CreateOnboardedArtistAsync(cityId: cityB.Id);

        await SetPreferenceAsync(caller.Id, maxDistance: 1, cityId: null);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(candidate.Id, "with no origin city the distance filter is bypassed");
    }

    // -------------------------------------------------------------------------
    // Edge
    // -------------------------------------------------------------------------

    // E1 — no UserMatchPreference row → 500.
    [Fact]
    public async Task GetArtists_NoPreferenceRow_Returns500()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        await Factory.ExecuteDbContextAsync(async db =>
            await db.UserMatchPreferences.Where(p => p.UserId == caller.Id).ExecuteDeleteAsync());

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // E2 — empty candidate pool → empty list.
    [Fact]
    public async Task GetArtists_EmptyPool_ReturnsEmpty()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        (await GetAsync(caller)).Should().BeEmpty();
    }

    // E3 — combined filters (country + age + gender + tags) narrow correctly.
    [Fact]
    public async Task GetArtists_CombinedFilters_NarrowCorrectly()
    {
        var cityA = await Factory.GetAnyCityAsync();
        var gender = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(1);
        var today = DateOnly.FromDateTime(DateTime.Today);

        var caller = await Factory.CreateOnboardedArtistAsync(countryId: cityA.CountryId, cityId: cityA.Id);

        var match = await Factory.CreateOnboardedArtistAsync(
            countryId: cityA.CountryId, cityId: cityA.Id, genderId: gender,
            birthDate: today.AddYears(-27), profileTagIds: tagIds);

        // Fails the age filter only.
        var tooYoung = await Factory.CreateOnboardedArtistAsync(
            countryId: cityA.CountryId, cityId: cityA.Id, genderId: gender,
            birthDate: today.AddYears(-18), profileTagIds: tagIds);

        await SetPreferenceAsync(caller.Id,
            countryId: cityA.CountryId, minAge: 25, maxAge: 40, genderId: gender, tagIds: tagIds);

        var ids = (await GetAsync(caller)).Select(r => r.Id).ToList();
        ids.Should().Contain(match.Id);
        ids.Should().NotContain(tooYoung.Id);
    }

    // -------------------------------------------------------------------------
    // CC-PAG
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetArtists_LimitZero_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 0), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetArtists_LimitTooLarge_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 51), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetArtists_NegativeOffset_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(offset: -1), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetArtists_BothInvalid_Returns422WithBothKeys()
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
    public async Task GetArtists_ValidBoundaries_Returns200(int limit, int offset)
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit, offset), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetArtists_NonIntegerLimit_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(new Uri($"{MatchingTestConstants.ArtistsRoute}?limit=abc&offset=0", UriKind.Relative), TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-PAG — stable ordering across pages.
    [Fact]
    public async Task GetArtists_Paging_StableOrderingNoSkipOrRepeat()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var created = new List<Guid>();
        for (var i = 0; i < 3; i++)
        {
            created.Add((await Factory.CreateOnboardedArtistAsync()).Id);
        }

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var page1 = await (await client.GetAsync(Route(limit: 2, offset: 0), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<OtherUserProfileArtistResponse>>();
        var page2 = await (await client.GetAsync(Route(limit: 2, offset: 2), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<OtherUserProfileArtistResponse>>();

        var seen = page1.Concat(page2).Select(p => p.Id).ToList();
        seen.Should().OnlyHaveUniqueItems();
        seen.Should().BeEquivalentTo(created);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetArtists_NoCredentials_Returns401()
    {
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_WrongAudienceToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongAudienceTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_CookieAuthReachesHandler_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        (await client.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetArtists_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetArtists_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
