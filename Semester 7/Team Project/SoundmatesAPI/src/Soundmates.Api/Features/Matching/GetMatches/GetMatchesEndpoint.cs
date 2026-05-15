using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Features.Users.GetOtherProfile;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.GetMatches;

internal static class GetMatchesEndpoint
{
    private const int MaxLimit = 50;

    public static IEndpointRouteBuilder MapGetMatches(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/matches", HandleAsync)
            .WithName("GetMatches")
            .WithSummary("Get all matches")
            .WithDescription("Returns a paginated list of all users the authenticated user has matched with.")
            .Produces<List<OtherUserProfileResponse>>(StatusCodes.Status200OK)
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

        var matches = await db.Matches
            .AsNoTracking()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Where(m => m.User1Id == user.Id || m.User2Id == user.Id)
            .OrderBy(m => m.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var userProfiles = new List<OtherUserProfileResponse>();

        foreach (var match in matches)
        {
            var otherUser = match.User1Id == user.Id ? match.User2 : match.User1;
            if (otherUser is null || !otherUser.IsActive || otherUser.IsFirstLogin || !otherUser.IsEmailConfirmed || otherUser.IsBand is null)
                continue;

            if ((bool)otherUser.IsBand)
            {
                var band = await db.Bands
                    .AsNoTracking()
                    .Include(b => b.Members)
                    .Include(b => b.User).ThenInclude(u => u.Tags)
                    .Include(b => b.User).ThenInclude(u => u.MusicSamples)
                    .Include(b => b.User).ThenInclude(u => u.ProfilePictures)
                    .FirstOrDefaultAsync(b => b.UserId == otherUser.Id, cancellationToken);

                if (band is null) continue;

                userProfiles.Add(new OtherUserProfileBandResponse
                {
                    Id = band.User.Id,
                    IsBand = band.User.IsBand,
                    Name = band.User.Name!,
                    Description = band.User.Description,
                    CountryId = (Guid)band.User.CountryId!,
                    CityId = (Guid)band.User.CityId!,
                    TagsIds = band.User.Tags.Select(t => t.Id).ToList(),
                    MusicSamples = band.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                        .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName))).ToList(),
                    ProfilePictures = band.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                        .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName))).ToList(),
                    BandMembers = band.Members.OrderBy(m => m.DisplayOrder)
                        .Select(bm => new BandMemberDto(bm.Name, bm.Age, bm.BandRoleId)).ToList()
                });
            }
            else
            {
                var artist = await db.Artists
                    .AsNoTracking()
                    .Include(a => a.User).ThenInclude(u => u.Tags)
                    .Include(a => a.User).ThenInclude(u => u.MusicSamples)
                    .Include(a => a.User).ThenInclude(u => u.ProfilePictures)
                    .FirstOrDefaultAsync(a => a.UserId == otherUser.Id, cancellationToken);

                if (artist is null) continue;

                userProfiles.Add(new OtherUserProfileArtistResponse
                {
                    Id = artist.User.Id,
                    IsBand = artist.User.IsBand,
                    Name = artist.User.Name!,
                    Description = artist.User.Description,
                    CountryId = (Guid)artist.User.CountryId!,
                    CityId = (Guid)artist.User.CityId!,
                    TagsIds = artist.User.Tags.Select(t => t.Id).ToList(),
                    MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                        .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName))).ToList(),
                    ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                        .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName))).ToList(),
                    BirthDate = artist.BirthDate
                });
            }
        }

        return TypedResults.Ok(userProfiles);
    }
}
