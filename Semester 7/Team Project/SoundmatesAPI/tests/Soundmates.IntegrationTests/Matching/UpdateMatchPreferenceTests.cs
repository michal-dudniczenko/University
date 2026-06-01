using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for PUT /matching/match-preference — UpdateMatchPreference (3.27 in tests-plan.md).
/// Authenticated · ValidationFilter&lt;UpdateMatchPreferenceRequest&gt; · CSRF · GetAuthorizedUserAsync(true).
/// </summary>
public sealed class UpdateMatchPreferenceTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static UpdateMatchPreferenceRequest DefaultRequest(
        bool showArtists = true,
        bool showBands = true,
        int? maxDistance = null,
        string? countryId = null,
        string? cityId = null,
        int? artistMinAge = null,
        int? artistMaxAge = null,
        string? artistGenderId = null,
        int? bandMin = null,
        int? bandMax = null,
        IList<string>? tags = null) =>
        new(showArtists, showBands, maxDistance, countryId, cityId, artistMinAge, artistMaxAge,
            artistGenderId, bandMin, bandMax, tags ?? []);

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — existing preference fully overwritten; tag set replaced; nulls stored as null.
    [Fact]
    public async Task UpdateMatchPreference_ExistingRow_OverwritesAllFields()
    {
        var country = await Factory.GetAnyCountryIdAsync();
        var city = await Factory.GetAnyCityAsync();
        var gender = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(2);

        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var request = DefaultRequest(
            showArtists: false,
            showBands: false,
            maxDistance: 50,
            countryId: country.ToString(),
            cityId: city.Id.ToString(),
            artistMinAge: 20,
            artistMaxAge: 35,
            artistGenderId: gender.ToString(),
            bandMin: 2,
            bandMax: 5,
            tags: tagIds.Select(t => t.ToString()).ToList());

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), request, TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences
                .AsNoTracking()
                .Include(p => p.Tags)
                .FirstAsync(p => p.UserId == caller.Id);
            pref.ShowArtists.Should().BeFalse();
            pref.ShowBands.Should().BeFalse();
            pref.MaxDistance.Should().Be(50);
            pref.CountryId.Should().Be(country);
            pref.CityId.Should().Be(city.Id);
            pref.ArtistMinAge.Should().Be(20);
            pref.ArtistMaxAge.Should().Be(35);
            pref.ArtistGenderId.Should().Be(gender);
            pref.BandMinMembersCount.Should().Be(2);
            pref.BandMaxMembersCount.Should().Be(5);
            pref.Tags.Select(t => t.Id).Should().BeEquivalentTo(tagIds);
        });
    }

    // H1 — nulls for CountryId/CityId/ArtistGenderId persisted as null.
    [Fact]
    public async Task UpdateMatchPreference_NullScalars_StoredAsNull()
    {
        var country = await Factory.GetAnyCountryIdAsync();
        var caller = await Factory.CreateOnboardedArtistAsync();

        // First set a non-null country so we can confirm it gets cleared.
        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.FirstAsync(p => p.UserId == caller.Id);
            pref.CountryId = country;
            await db.SaveChangesAsync();
        });

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking().FirstAsync(p => p.UserId == caller.Id);
            pref.CountryId.Should().BeNull();
            pref.CityId.Should().BeNull();
            pref.ArtistGenderId.Should().BeNull();
        });
    }

    // H2 — no existing preference row → 200; a new row is created with the values.
    [Fact]
    public async Task UpdateMatchPreference_NoExistingRow_CreatesNew()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();

        await Factory.ExecuteDbContextAsync(async db =>
            await db.UserMatchPreferences.Where(p => p.UserId == caller.Id).ExecuteDeleteAsync());

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var request = DefaultRequest(showArtists: false, showBands: true, maxDistance: 75);

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), request, TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking().FirstAsync(p => p.UserId == caller.Id);
            pref.ShowArtists.Should().BeFalse();
            pref.ShowBands.Should().BeTrue();
            pref.MaxDistance.Should().Be(75);
        });
    }

    // H3 — duplicate FilterTagsIds are de-duplicated and stored once each.
    [Fact]
    public async Task UpdateMatchPreference_DuplicateTagIds_StoredOnceEach()
    {
        var tagIds = await Factory.GetArtistTagIdsAsync(2);
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var withDuplicates = new List<string>
        {
            tagIds[0].ToString(), tagIds[0].ToString(), tagIds[1].ToString(), tagIds[1].ToString()
        };

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(tags: withDuplicates), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking()
                .Include(p => p.Tags).FirstAsync(p => p.UserId == caller.Id);
            pref.Tags.Select(t => t.Id).Should().BeEquivalentTo(tagIds);
        });
    }

    // H4 — empty FilterTagsIds clears the tag set (no DB lookup performed).
    [Fact]
    public async Task UpdateMatchPreference_EmptyTagList_ClearsTags()
    {
        var tagIds = await Factory.GetArtistTagIdsAsync(2);
        var caller = await Factory.CreateOnboardedArtistAsync();

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.Include(p => p.Tags).FirstAsync(p => p.UserId == caller.Id);
            var tags = await db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
            foreach (var tag in tags) pref.Tags.Add(tag);
            await db.SaveChangesAsync();
        });

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(tags: []), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking()
                .Include(p => p.Tags).FirstAsync(p => p.UserId == caller.Id);
            pref.Tags.Should().BeEmpty();
        });
    }

    // -------------------------------------------------------------------------
    // Validation (CC-VAL)
    // -------------------------------------------------------------------------

    // V1 — FilterTagsIds null → 422.
    [Fact]
    public async Task UpdateMatchPreference_NullTagList_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        // Serialize the body manually so FilterTagsIds is JSON null.
        const string body = """
        {"showArtists":true,"showBands":true,"maxDistance":null,"countryId":null,"cityId":null,
        "artistMinAge":null,"artistMaxAge":null,"artistGenderId":null,"bandMinMembersCount":null,
        "bandMaxMembersCount":null,"filterTagsIds":null}
        """;

        var response = await client.PutAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative),
            new StringContent(body, Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V2 — an empty FilterTagsIds element → 422.
    [Fact]
    public async Task UpdateMatchPreference_EmptyTagElement_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(tags: [string.Empty]), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V2 — a non-GUID FilterTagsIds element → 422.
    [Fact]
    public async Task UpdateMatchPreference_NonGuidTagElement_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(tags: ["not-a-guid"]), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V3 — CountryId present but not a GUID → 422.
    [Fact]
    public async Task UpdateMatchPreference_NonGuidCountryId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(countryId: "not-a-guid"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V4 — CityId present but not a GUID → 422.
    [Fact]
    public async Task UpdateMatchPreference_NonGuidCityId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(cityId: "not-a-guid"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // V5 — ArtistGenderId present but not a GUID → 422.
    [Fact]
    public async Task UpdateMatchPreference_NonGuidArtistGenderId_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(artistGenderId: "not-a-guid"), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-VAL-4 — malformed JSON → 400.
    [Fact]
    public async Task UpdateMatchPreference_MalformedJson_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), new StringContent("{bad", Encoding.UTF8, "application/json"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // CC-VAL-5 — wrong Content-Type → 415.
    [Fact]
    public async Task UpdateMatchPreference_WrongContentType_Returns415()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), new StringContent("x", Encoding.UTF8, "text/plain"), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    // -------------------------------------------------------------------------
    // Handler failures / edge
    // -------------------------------------------------------------------------

    // F1 — a well-formed but nonexistent tag GUID → 500.
    [Fact]
    public async Task UpdateMatchPreference_NonexistentTagId_Returns500()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            MatchingTestConstants.MatchPreferenceRoute,
            DefaultRequest(tags: [Guid.NewGuid().ToString()]), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // E1 — no numeric validation: negative MaxDistance / inverted ages / inverted member counts → 200.
    [Fact]
    public async Task UpdateMatchPreference_SkewingNumericValues_Accepted()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var request = DefaultRequest(
            maxDistance: -10,
            artistMinAge: 50,
            artistMaxAge: 18,
            bandMin: 9,
            bandMax: 1);

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), request, TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking().FirstAsync(p => p.UserId == caller.Id);
            pref.MaxDistance.Should().Be(-10);
            pref.ArtistMinAge.Should().Be(50);
            pref.ArtistMaxAge.Should().Be(18);
            pref.BandMinMembersCount.Should().Be(9);
            pref.BandMaxMembersCount.Should().Be(1);
        });
    }

    // E2 — well-formed but nonexistent CountryId/CityId/ArtistGenderId stored without FK checks → 200.
    // (No navigation FK is enforced on the preference for these columns.)
    [Fact]
    public async Task UpdateMatchPreference_NonexistentScalarGuids_Accepted()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var country = Guid.NewGuid();
        var city = Guid.NewGuid();
        var gender = Guid.NewGuid();

        var request = DefaultRequest(
            countryId: country.ToString(),
            cityId: city.ToString(),
            artistGenderId: gender.ToString());

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), request, TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences.AsNoTracking().FirstAsync(p => p.UserId == caller.Id);
            pref.CountryId.Should().Be(country);
            pref.CityId.Should().Be(city);
            pref.ArtistGenderId.Should().Be(gender);
        });
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UpdateMatchPreference_NoCredentials_Returns401()
    {
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_WrongIssuerToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongIssuerTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // CC-AUTH-6 — JWT and cookie both reach the handler.
    [Fact]
    public async Task UpdateMatchPreference_JwtAndCookieBothReachHandler()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();

        var jwtClient = await Factory.CreateAuthenticatedClientAsync(caller);
        (await jwtClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var cookieClient = await Factory.CreateCookieClientAsync(caller);
        (await cookieClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken))
            .StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA (checkForFirstLogin: true)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UpdateMatchPreference_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMatchPreference_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // CC-CSRF
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UpdateMatchPreference_CookieAuthMissingCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateMatchPreference_CookieAuthInvalidCsrf_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        client.DefaultRequestHeaders.Add(TestConstants.CsrfTokenHeaderName, "bogus");

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateMatchPreference_CookieAuthValidCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: true);

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateMatchPreference_JwtAuthSkipsCsrf_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PutAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // Routing
    // -------------------------------------------------------------------------

    [Fact]
    public async Task UpdateMatchPreference_WrongMethod_Returns405()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.PostAsJsonAsync(
            new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), DefaultRequest(), TestJson.Options, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
}
