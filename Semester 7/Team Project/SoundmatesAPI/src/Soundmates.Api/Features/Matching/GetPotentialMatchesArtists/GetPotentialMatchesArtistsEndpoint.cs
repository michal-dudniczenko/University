using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Features.Users.GetOtherProfile;
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
            .WithTags("Matching")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = PaginationValidator.ValidateLimitOffset(limit, offset, MaxLimit);
        if (errors is not null)
        {
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));
        }

        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var userMatchPreference = await db.UserMatchPreferences
            .AsNoTracking()
            .Include(ump => ump.User).ThenInclude(u => u.City)
            .Include(ump => ump.Tags).ThenInclude(t => t.TagCategory)
            .FirstOrDefaultAsync(ump => ump.UserId == user.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Match preference data for user: {user.Id} not found.");

        if (!userMatchPreference.ShowArtists)
            return TypedResults.Ok(new List<OtherUserProfileArtistResponse>());

        var likedUserIds = await db.Likes.AsNoTracking()
            .Where(l => l.GiverId == user.Id).Select(l => l.ReceiverId).ToListAsync(cancellationToken);
        var dislikedUserIds = await db.Dislikes.AsNoTracking()
            .Where(d => d.GiverId == user.Id).Select(d => d.ReceiverId).ToListAsync(cancellationToken);

        IQueryable<Artist> artists = db.Artists
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Tags)
            .Include(a => a.User).ThenInclude(u => u.MusicSamples)
            .Include(a => a.User).ThenInclude(u => u.ProfilePictures)
            .Include(a => a.User).ThenInclude(u => u.City);

        artists = artists.Where(a =>
            a.User.IsActive && a.User.IsEmailConfirmed && !a.User.IsFirstLogin && a.User.Id != user.Id &&
            !likedUserIds.Contains(a.User.Id) && !dislikedUserIds.Contains(a.User.Id));

        var originCity = userMatchPreference.User.City;

        if (userMatchPreference.MaxDistance is not null && originCity is not null)
        {
            artists = artists.Where(a => a.User.City != null);
            artists = artists.Where(a =>
                HaversineDistance(originCity.Latitude, originCity.Longitude, a.User.City!.Latitude, a.User.City!.Longitude)
                <= userMatchPreference.MaxDistance.Value);
        }

        if (userMatchPreference.CountryId is not null)
            artists = artists.Where(a => a.User.CountryId == userMatchPreference.CountryId);

        if (userMatchPreference.CityId is not null)
            artists = artists.Where(a => a.User.CityId == userMatchPreference.CityId);

        if (userMatchPreference.ArtistMinAge is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var minAgeCutoff = today.AddYears(-userMatchPreference.ArtistMinAge.Value);
            artists = artists.Where(a => a.BirthDate <= minAgeCutoff);
        }

        if (userMatchPreference.ArtistMaxAge is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxAgeCutoff = today.AddYears(-(userMatchPreference.ArtistMaxAge.Value + 1));
            artists = artists.Where(a => a.BirthDate > maxAgeCutoff);
        }

        if (userMatchPreference.ArtistGenderId is not null)
            artists = artists.Where(a => a.GenderId == userMatchPreference.ArtistGenderId);

        foreach (var tag in userMatchPreference.Tags.Where(t => !t.TagCategory.IsForBand))
            artists = artists.Where(a => a.User.Tags.Any(t => t.Id == tag.Id));

        var maxDistance = userMatchPreference.MaxDistance;
        var preferenceTagIds = userMatchPreference.Tags.Where(t => !t.TagCategory.IsForBand).Select(t => t.Id).ToList();

        var result = await artists
            .OrderByDescending(a =>
                (a.User.Tags.Count(t => preferenceTagIds.Contains(t.Id)) * 100.0) +
                (originCity == null || a.User.City == null || maxDistance == null || maxDistance.Value == 0
                    ? 0.0
                    : (1.0 - (HaversineDistance(originCity.Latitude, originCity.Longitude, a.User.City.Latitude, a.User.City.Longitude) / maxDistance.Value)) * 100.0))
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var dtos = result.Select(a => new OtherUserProfileArtistResponse
        {
            Id = a.User.Id,
            IsBand = a.User.IsBand,
            Name = a.User.Name!,
            Description = a.User.Description,
            CountryId = (Guid)a.User.CountryId!,
            CityId = (Guid)a.User.CityId!,
            TagsIds = a.User.Tags.Select(t => t.Id).ToList(),
            MusicSamples = a.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName))).ToList(),
            ProfilePictures = a.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName))).ToList(),
            BirthDate = a.BirthDate
        }).ToList();

        return TypedResults.Ok(dtos);
    }

    public static double HaversineDistance(double originLat, double originLon, double destLat, double destLon)
    {
        const double earthRadiusKm = 6371.0;
        double originLatRad = originLat * (Math.PI / 180.0);
        double originLonRad = originLon * (Math.PI / 180.0);
        double destLatRad = destLat * (Math.PI / 180.0);
        double destLonRad = destLon * (Math.PI / 180.0);

        double dLat = (destLatRad - originLatRad) / 2.0;
        double dLon = (destLonRad - originLonRad) / 2.0;
        double a = Math.Pow(Math.Sin(dLat), 2.0) +
                   Math.Cos(originLatRad) * Math.Cos(destLatRad) *
                   Math.Pow(Math.Sin(dLon), 2.0);

        double c = 2.0 * Math.Asin(Math.Sqrt(a));
        return earthRadiusKm * c;
    }
}
