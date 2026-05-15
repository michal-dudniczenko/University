using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Matching.GetPotentialMatchesArtists;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Features.Users.GetOtherProfile;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.GetPotentialMatchesBands;

internal static class GetPotentialMatchesBandsEndpoint
{
    private const int MaxLimit = 50;

    public static IEndpointRouteBuilder MapGetPotentialMatchesBands(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/bands", HandleAsync)
            .WithName("GetPotentialMatchesBands")
            .WithSummary("Get potential band matches")
            .WithDescription("Returns a paginated list of band profiles that match the authenticated user's preferences.")
            .Produces<List<OtherUserProfileBandResponse>>(StatusCodes.Status200OK)
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

        if (!userMatchPreference.ShowBands)
            return TypedResults.Ok(new List<OtherUserProfileBandResponse>());

        var likedUserIds = await db.Likes.AsNoTracking()
            .Where(l => l.GiverId == user.Id).Select(l => l.ReceiverId).ToListAsync(cancellationToken);
        var dislikedUserIds = await db.Dislikes.AsNoTracking()
            .Where(d => d.GiverId == user.Id).Select(d => d.ReceiverId).ToListAsync(cancellationToken);

        IQueryable<Band> bands = db.Bands
            .AsNoTracking()
            .Include(b => b.Members)
            .Include(b => b.User).ThenInclude(u => u.Tags)
            .Include(b => b.User).ThenInclude(u => u.MusicSamples)
            .Include(b => b.User).ThenInclude(u => u.ProfilePictures)
            .Include(b => b.User).ThenInclude(u => u.City);

        bands = bands.Where(b =>
            b.User.IsActive && b.User.IsEmailConfirmed && !b.User.IsFirstLogin && b.User.Id != user.Id &&
            !likedUserIds.Contains(b.User.Id) && !dislikedUserIds.Contains(b.User.Id));

        var originCity = userMatchPreference.User.City;

        if (userMatchPreference.MaxDistance is not null && originCity is not null)
        {
            bands = bands.Where(b => b.User.City != null);
            bands = bands.Where(b =>
                GetPotentialMatchesArtistsEndpoint.HaversineDistance(originCity.Latitude, originCity.Longitude, b.User.City!.Latitude, b.User.City!.Longitude)
                <= userMatchPreference.MaxDistance.Value);
        }

        if (userMatchPreference.CountryId is not null)
            bands = bands.Where(b => b.User.CountryId == userMatchPreference.CountryId);

        if (userMatchPreference.CityId is not null)
            bands = bands.Where(b => b.User.CityId == userMatchPreference.CityId);

        if (userMatchPreference.BandMinMembersCount is not null)
            bands = bands.Where(b => b.Members.Count >= userMatchPreference.BandMinMembersCount);

        if (userMatchPreference.BandMaxMembersCount is not null)
            bands = bands.Where(b => b.Members.Count <= userMatchPreference.BandMaxMembersCount);

        foreach (var tag in userMatchPreference.Tags.Where(t => t.TagCategory.IsForBand))
            bands = bands.Where(b => b.User.Tags.Any(t => t.Id == tag.Id));

        var maxDistance = userMatchPreference.MaxDistance;
        var preferenceTagIds = userMatchPreference.Tags.Where(t => !t.TagCategory.IsForBand).Select(t => t.Id).ToList();

        var result = await bands
            .OrderByDescending(b =>
                (b.User.Tags.Count(t => preferenceTagIds.Contains(t.Id)) * 100.0) +
                (originCity == null || b.User.City == null || maxDistance == null || maxDistance.Value == 0
                    ? 0.0
                    : (1.0 - (GetPotentialMatchesArtistsEndpoint.HaversineDistance(originCity.Latitude, originCity.Longitude, b.User.City.Latitude, b.User.City.Longitude) / maxDistance.Value)) * 100.0))
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var dtos = result.Select(b => new OtherUserProfileBandResponse
        {
            Id = b.User.Id,
            IsBand = b.User.IsBand,
            Name = b.User.Name!,
            Description = b.User.Description,
            CountryId = (Guid)b.User.CountryId!,
            CityId = (Guid)b.User.CityId!,
            TagsIds = b.User.Tags.Select(t => t.Id).ToList(),
            MusicSamples = b.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName))).ToList(),
            ProfilePictures = b.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName))).ToList(),
            BandMembers = b.Members.OrderBy(m => m.DisplayOrder)
                .Select(bm => new BandMemberDto(bm.Name, bm.Age, bm.BandRoleId)).ToList()
        }).ToList();

        return TypedResults.Ok(dtos);
    }
}
