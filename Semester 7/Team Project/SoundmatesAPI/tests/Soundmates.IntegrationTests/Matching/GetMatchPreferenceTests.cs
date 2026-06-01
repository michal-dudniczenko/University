using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for GET /matching/match-preference — GetMatchPreference (3.23 in tests-plan.md).
/// Authenticated · GetAuthorizedUserAsync(true).
/// </summary>
public sealed class GetMatchPreferenceTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

    // H1 — newly onboarded user → 200 with the default preference values.
    [Fact]
    public async Task GetMatchPreference_NewlyOnboarded_ReturnsDefaults()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.ReadRequiredAsync<MatchPreferenceResponse>();
        body.ShowArtists.Should().BeTrue();
        body.ShowBands.Should().BeTrue();
        body.MaxDistance.Should().BeNull();
        body.CountryId.Should().BeNull();
        body.CityId.Should().BeNull();
        body.ArtistMinAge.Should().BeNull();
        body.ArtistMaxAge.Should().BeNull();
        body.ArtistGenderId.Should().BeNull();
        body.BandMinMembersCount.Should().BeNull();
        body.BandMaxMembersCount.Should().BeNull();
        body.FilterTagsIds.Should().BeEmpty();
    }

    // H1 — preference mirrors stored values incl. filter tag ids.
    [Fact]
    public async Task GetMatchPreference_WithStoredValues_MirrorsThem()
    {
        var country = await Factory.GetAnyCountryIdAsync();
        var city = await Factory.GetAnyCityAsync();
        var gender = await Factory.GetAnyGenderIdAsync();
        var tagIds = await Factory.GetArtistTagIdsAsync(2);

        var caller = await Factory.CreateOnboardedArtistAsync();

        await Factory.ExecuteDbContextAsync(async db =>
        {
            var pref = await db.UserMatchPreferences
                .Include(p => p.Tags)
                .FirstAsync(p => p.UserId == caller.Id);
            pref.ShowArtists = false;
            pref.ShowBands = true;
            pref.MaxDistance = 100;
            pref.CountryId = country;
            pref.CityId = city.Id;
            pref.ArtistMinAge = 18;
            pref.ArtistMaxAge = 40;
            pref.ArtistGenderId = gender;
            pref.BandMinMembersCount = 2;
            pref.BandMaxMembersCount = 6;
            var tags = await db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
            foreach (var tag in tags) pref.Tags.Add(tag);
            await db.SaveChangesAsync();
        });

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.ReadRequiredAsync<MatchPreferenceResponse>();
        body.ShowArtists.Should().BeFalse();
        body.ShowBands.Should().BeTrue();
        body.MaxDistance.Should().Be(100);
        body.CountryId.Should().Be(country);
        body.CityId.Should().Be(city.Id);
        body.ArtistMinAge.Should().Be(18);
        body.ArtistMaxAge.Should().Be(40);
        body.ArtistGenderId.Should().Be(gender);
        body.BandMinMembersCount.Should().Be(2);
        body.BandMaxMembersCount.Should().Be(6);
        body.FilterTagsIds.Should().BeEquivalentTo(tagIds);
    }

    // -------------------------------------------------------------------------
    // Edge
    // -------------------------------------------------------------------------

    // E1 — no preference row for the user → 500. Reachable by deleting the row.
    [Fact]
    public async Task GetMatchPreference_NoPreferenceRow_Returns500()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();

        await Factory.ExecuteDbContextAsync(async db =>
            await db.UserMatchPreferences.Where(p => p.UserId == caller.Id).ExecuteDeleteAsync());

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMatchPreference_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_WrongAudienceToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongAudienceTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_CookieAuthReachesHandler_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);

        var response = await client.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA (checkForFirstLogin: true)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMatchPreference_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatchPreference_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        var response = await HttpClient.GetAsync(new Uri(MatchingTestConstants.MatchPreferenceRoute, UriKind.Relative), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // -------------------------------------------------------------------------
    // Routing
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMatchPreference_UnknownRoute_Returns404()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(new Uri("/matching/match-preference/extra", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
