using Microsoft.EntityFrameworkCore;
using Soundmates.IntegrationTests.Matching.Contracts;
using System.Net;

namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Tests for GET /matching/matches?limit&amp;offset — GetMatches (3.22 in tests-plan.md).
/// Authenticated · GetAuthorizedUserAsync(true) · pagination (maxLimit 50).
/// </summary>
public sealed class GetMatchesTests(CustomWebApplicationFactory factory)
    : IntegrationTestBase(factory)
{
    private static Uri Route(int limit = 50, int offset = 0) =>
        new($"{MatchingTestConstants.MatchesRoute}?limit={limit}&offset={offset}", UriKind.Relative);

    // -------------------------------------------------------------------------
    // Happy paths
    // -------------------------------------------------------------------------

    // H1 — caller with matches → list of the OTHER user in each match (both positions checked).
    [Fact]
    public async Task GetMatches_WithMatches_ReturnsOtherUsersBothPositions()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var artist = await Factory.CreateOnboardedArtistAsync(name: "Matched Artist");
        var band = await Factory.CreateOnboardedBandAsync(name: "Matched Band");

        // caller is User1 with the artist; caller is User2 with the band.
        await Factory.SeedMatchAsync(caller.Id, artist.Id);
        await Factory.SeedMatchAsync(band.Id, caller.Id);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.ReadRequiredAsync<List<GetOtherUserProfileResponse>>();
        body.Should().HaveCount(2);
        body.Select(p => p.Id).Should().BeEquivalentTo(new[] { artist.Id, band.Id });

        var artistProfile = body.Single(p => p.Id == artist.Id);
        artistProfile.Should().BeOfType<OtherUserProfileArtistResponse>();
        ((OtherUserProfileArtistResponse)artistProfile).BirthDate.Should().NotBeNull();

        var bandProfile = body.Single(p => p.Id == band.Id);
        bandProfile.Should().BeOfType<OtherUserProfileBandResponse>();
        ((OtherUserProfileBandResponse)bandProfile).BandMembers.Should().NotBeEmpty();
    }

    // H2 — no matches → empty list.
    [Fact]
    public async Task GetMatches_NoMatches_ReturnsEmpty()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route(), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<List<GetOtherUserProfileResponse>>()).Should().BeEmpty();
    }

    // E3 — media URLs are absolute and ordered by DisplayOrder.
    [Fact]
    public async Task GetMatches_MediaUrlsAbsoluteAndOrdered()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        await Factory.SeedProfilePictureAsync(other.Id, "second.jpg", 1);
        await Factory.SeedProfilePictureAsync(other.Id, "first.jpg", 0);
        await Factory.SeedMusicSampleAsync(other.Id, "track.mp3", 0);

        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.ReadRequiredAsync<List<GetOtherUserProfileResponse>>();
        var profile = body.Single(p => p.Id == other.Id);

        profile.ProfilePictures.Should().HaveCount(2);
        profile.ProfilePictures[0].FileUrl.Should().Contain("first.jpg");
        profile.ProfilePictures[1].FileUrl.Should().Contain("second.jpg");
        profile.ProfilePictures.Should().AllSatisfy(p =>
            Uri.IsWellFormedUriString(p.FileUrl, UriKind.Absolute).Should().BeTrue());
        profile.MusicSamples.Should().ContainSingle()
            .Which.FileUrl.Should().StartWith("http");
    }

    // -------------------------------------------------------------------------
    // Edge: excluded counterparts (E1)
    // -------------------------------------------------------------------------

    // E1 — other user inactive → excluded from results, match row still present.
    [Fact]
    public async Task GetMatches_OtherUserInactive_Excluded()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == other.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsActive, false)));

        await AssertMatchExcludedButRowSurvives(caller);
    }

    // E1 — other user first-login → excluded.
    [Fact]
    public async Task GetMatches_OtherUserFirstLogin_Excluded()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == other.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsFirstLogin, true)));

        await AssertMatchExcludedButRowSurvives(caller);
    }

    // E1 — other user unconfirmed → excluded.
    [Fact]
    public async Task GetMatches_OtherUserUnconfirmed_Excluded()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == other.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.EmailConfirmed, false)));

        await AssertMatchExcludedButRowSurvives(caller);
    }

    // E1 — other user IsBand == null → excluded.
    [Fact]
    public async Task GetMatches_OtherUserIsBandNull_Excluded()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var other = await Factory.CreateOnboardedArtistAsync();
        await Factory.SeedMatchAsync(caller.Id, other.Id);

        await Factory.ExecuteDbContextAsync(async db =>
            await db.Users.Where(u => u.Id == other.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsBand, (bool?)null)));

        await AssertMatchExcludedButRowSurvives(caller);
    }

    private async Task AssertMatchExcludedButRowSurvives(TestUser caller)
    {
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        var response = await client.GetAsync(Route(), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.ReadRequiredAsync<List<GetOtherUserProfileResponse>>())
            .Should().BeEmpty("the ineligible counterpart must be filtered out of the results");

        await Factory.ExecuteDbContextAsync(async db =>
            (await db.Matches.CountAsync()).Should().Be(1, "the match row itself must not be deleted"));
    }

    // -------------------------------------------------------------------------
    // Pagination ordering (E2)
    // -------------------------------------------------------------------------

    // E2 — deterministic order across pages (no skip / repeat) when paging through > limit.
    [Fact]
    public async Task GetMatches_Paging_NoSkipOrRepeat()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var others = new List<Guid>();
        for (var i = 0; i < 3; i++)
        {
            var other = await Factory.CreateOnboardedArtistAsync();
            await Factory.SeedMatchAsync(caller.Id, other.Id);
            others.Add(other.Id);
        }

        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var page1 = await (await client.GetAsync(Route(limit: 2, offset: 0), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<GetOtherUserProfileResponse>>();
        var page2 = await (await client.GetAsync(Route(limit: 2, offset: 2), TestContext.Current.CancellationToken))
            .ReadRequiredAsync<List<GetOtherUserProfileResponse>>();

        page1.Should().HaveCount(2);
        page2.Should().HaveCount(1);

        var seen = page1.Concat(page2).Select(p => p.Id).ToList();
        seen.Should().OnlyHaveUniqueItems();
        seen.Should().BeEquivalentTo(others);
    }

    // -------------------------------------------------------------------------
    // CC-PAG
    // -------------------------------------------------------------------------

    // CC-PAG-1 — limit <= 0 → 422.
    [Fact]
    public async Task GetMatches_LimitZero_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 0), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-PAG-2 — limit > 50 → 422.
    [Fact]
    public async Task GetMatches_LimitTooLarge_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit: 51), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-PAG-3 — offset < 0 → 422.
    [Fact]
    public async Task GetMatches_NegativeOffset_Returns422()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(offset: -1), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // CC-PAG-4 — both invalid → 422 with both keys.
    [Fact]
    public async Task GetMatches_BothInvalid_Returns422WithBothKeys()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(Route(limit: 0, offset: -1), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.ReadRequiredAsync<TestValidationProblem>();
        problem.Errors.Should().ContainKey(MatchingTestConstants.LimitErrorKey);
        problem.Errors.Should().ContainKey(MatchingTestConstants.OffsetErrorKey);
    }

    // CC-PAG-5 — valid boundaries succeed.
    [Theory]
    [InlineData(1, 0)]
    [InlineData(50, 0)]
    public async Task GetMatches_ValidBoundaries_Returns200(int limit, int offset)
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);
        (await client.GetAsync(Route(limit, offset), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // CC-PAG-6 — non-integer query value → 400 (binding).
    [Fact]
    public async Task GetMatches_NonIntegerLimit_Returns400()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateAuthenticatedClientAsync(caller);

        var response = await client.GetAsync(new Uri($"{MatchingTestConstants.MatchesRoute}?limit=abc&offset=0", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // -------------------------------------------------------------------------
    // CC-AUTH
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMatches_NoCredentials_Returns401()
    {
        var response = await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_GarbageToken_Returns401()
    {
        HttpClient.SetBearerToken("garbage");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_ExpiredToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintExpiredTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_WrongKeyToken_Returns401()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        HttpClient.SetBearerToken(await Factory.MintWrongKeyTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_InvalidAuthCookie_Returns401()
    {
        HttpClient.DefaultRequestHeaders.Add("Cookie", $"{TestConstants.AuthCookieName}=invalid");
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_CookieAuthReachesHandler_Returns200()
    {
        var caller = await Factory.CreateOnboardedArtistAsync();
        var client = await Factory.CreateCookieClientAsync(caller, attachCsrf: false);
        (await client.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // -------------------------------------------------------------------------
    // CC-GA
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetMatches_TokenForDeletedUser_Returns401()
    {
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(Guid.NewGuid(), "ghost@test.local"));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_UnconfirmedCaller_Returns401()
    {
        var caller = await Factory.CreateUnconfirmedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_DeactivatedCaller_Returns401()
    {
        var caller = await Factory.CreateDeactivatedUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMatches_FirstLoginCaller_Returns401()
    {
        var caller = await Factory.CreateFirstLoginUserAsync();
        HttpClient.SetBearerToken(await Factory.MintTokenAsync(caller.Id, caller.Email));
        (await HttpClient.GetAsync(Route(), TestContext.Current.CancellationToken)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
