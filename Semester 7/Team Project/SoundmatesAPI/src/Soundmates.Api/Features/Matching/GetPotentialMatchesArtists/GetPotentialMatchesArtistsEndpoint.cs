using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Common;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.GetPotentialMatchesArtists;

internal static class GetPotentialMatchesArtistsEndpoint
{
    private const int MaxLimit = 50;

    public static IEndpointRouteBuilder MapGetPotentialMatchesArtists(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/artists", HandleAsync)
            .WithName("GetPotentialMatchesArtists")
            .WithSummary("Get potential artist matches")
            .WithDescription("Returns a paginated list of artist profiles that match the authenticated user's preferences.")
            .Produces<List<OtherUserProfileArtistResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Matching");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        HttpRequest httpRequest,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = PaginationValidator.ValidateLimitOffset(limit, offset, MaxLimit);
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var userMatchPreference = await db.UserMatchPreferences
            .AsNoTracking()
            .Include(ump => ump.User)
                .ThenInclude(u => u.City)
            .Include(ump => ump.Tags)
                .ThenInclude(t => t.TagCategory)
            .FirstOrDefaultAsync(ump => ump.UserId == user.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Match preference data for user: {user.Id} not found.");

        if (!userMatchPreference.ShowArtists)
            return TypedResults.Ok(new List<OtherUserProfileArtistResponse>());

        var candidates = db.Artists
            .AsNoTracking()
            .Where(a =>
                a.User.IsActive && a.User.EmailConfirmed && !a.User.IsFirstLogin && a.UserId != user.Id
                && !db.Likes.Any(l => l.GiverId == user.Id && l.ReceiverId == a.UserId)
                && !db.Dislikes.Any(d => d.GiverId == user.Id && d.ReceiverId == a.UserId));

        if (userMatchPreference.CountryId is not null)
            candidates = candidates.Where(a => a.User.CountryId == userMatchPreference.CountryId);

        if (userMatchPreference.CityId is not null)
            candidates = candidates.Where(a => a.User.CityId == userMatchPreference.CityId);

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (userMatchPreference.ArtistMinAge is not null)
        {
            var minAgeCutoff = today.AddYears(-userMatchPreference.ArtistMinAge.Value);
            candidates = candidates.Where(a => a.BirthDate <= minAgeCutoff);
        }

        if (userMatchPreference.ArtistMaxAge is not null)
        {
            var maxAgeCutoff = today.AddYears(-(userMatchPreference.ArtistMaxAge.Value + 1));
            candidates = candidates.Where(a => a.BirthDate > maxAgeCutoff);
        }

        if (userMatchPreference.ArtistGenderId is not null)
            candidates = candidates.Where(a => a.GenderId == userMatchPreference.ArtistGenderId);

        // Distance, scoring, ordering and pagination all run on the database engine.
        // EF Core's SQL Server provider translates these Math/double calls to SIN/COS/ASIN/SQRT/POWER/RADIANS.
        // The Haversine formula must be inlined here rather than extracted into a (non-translatable) method.
        var originCity = userMatchPreference.User.City;
        var maxDistance = userMatchPreference.MaxDistance;
        var applyDistanceFilter = originCity is not null && maxDistance is not null;
        var applyDistanceScore = applyDistanceFilter && maxDistance!.Value != 0;

        var originLatRad = double.DegreesToRadians(originCity?.Latitude ?? 0);
        var originLonRad = double.DegreesToRadians(originCity?.Longitude ?? 0);
        var cosOriginLat = Math.Cos(originLatRad);

        var preferenceTagIds = userMatchPreference.Tags
            .Where(t => !t.TagCategory.IsForBand)
            .Select(t => t.Id)
            .ToList();
        var applyTagsFilter = preferenceTagIds.Count > 0;

        var projected = candidates.Select(a => new
        {
            a.User.Id,
            a.User.IsBand,
            a.User.Name,
            a.User.ProfileDescription,
            a.User.CountryId,
            a.User.CityId,
            a.BirthDate,
            TagIds = a.User.Tags
                .Select(t => t.Id)
                .ToList(),
            MusicSamples = a.User.MusicSamples
                .OrderBy(ms => ms.DisplayOrder)
                .Select(ms => new { ms.Id, ms.FileName })
                .ToList(),
            ProfilePictures = a.User.ProfilePictures
                .OrderBy(pp => pp.DisplayOrder)
                .Select(pp => new { pp.Id, pp.FileName })
                .ToList(),
            TagMatchCount = applyTagsFilter
                ? a.User.Tags.Count(t => preferenceTagIds.Contains(t.Id))
                : 0,
            Distance = originCity == null || a.User.City == null
                ? (double?)null
                : 2.0 * ApplicationConstants.EarthRadiusKm * Math.Asin(Math.Sqrt(
                    Math.Pow(Math.Sin((double.DegreesToRadians(a.User.City.Latitude) - originLatRad) / 2.0), 2.0)
                    + cosOriginLat * Math.Cos(double.DegreesToRadians(a.User.City.Latitude))
                    * Math.Pow(Math.Sin((double.DegreesToRadians(a.User.City.Longitude) - originLonRad) / 2.0), 2.0)))
        });

        if (applyTagsFilter)
            projected = projected.Where(x => x.TagMatchCount > 0);

        if (applyDistanceFilter)
            projected = projected.Where(x => x.Distance <= maxDistance!.Value);

        var ordered = applyDistanceScore
            ? projected.OrderByDescending(x =>
                (x.TagMatchCount * 100.0)
                + (x.Distance == null ? 0.0 : (1.0 - (x.Distance.Value / maxDistance!.Value)) * 100.0))
            : projected.OrderByDescending(x => x.TagMatchCount);

        // Id tiebreaker keeps ordering deterministic so pagination doesn't skip or repeat rows.
        // Split query avoids a cartesian explosion across the projected collections.
        var page = await ordered
            .ThenBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var dtos = page
            .Select(x => new OtherUserProfileArtistResponse
            {
                Id = x.Id,
                IsBand = x.IsBand,
                Name = x.Name
                    ?? throw new InvalidOperationException($"Active user should NOT have Name = NULL. User id: {x.Id}"),
                ProfileDescription = x.ProfileDescription,
                CountryId = x.CountryId is not null
                    ? (Guid)x.CountryId
                    : throw new InvalidOperationException($"Active user should NOT have CountryId = NULL. User id: {x.Id}"),
                CityId = x.CityId is not null
                    ? (Guid)x.CityId
                    : throw new InvalidOperationException($"Active user should NOT have CityId = NULL. User id: {x.Id}"),
                TagsIds = x.TagIds,
                MusicSamples = x.MusicSamples
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName, httpRequest)))
                    .ToList(),
                ProfilePictures = x.ProfilePictures
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName, httpRequest)))
                    .ToList(),
                BirthDate = x.BirthDate
            })
            .ToList();

        return TypedResults.Ok(dtos);
    }
}
