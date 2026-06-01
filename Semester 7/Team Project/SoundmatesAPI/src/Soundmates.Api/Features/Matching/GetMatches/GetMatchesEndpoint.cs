using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Common;
using Soundmates.Api.Features.Users.Common;
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
            .WithTags("Matching");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var errors = PaginationValidator.ValidateLimitOffset(limit, offset, MaxLimit);
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        // Page over matches first, projecting only the *other* user's id (a scalar ternary EF can
        // translate). Selecting the other user as an entity via a ternary and then sub-projecting
        // its collections does not translate, so the user profiles are loaded in a second query.
        var orderedOtherUserIds = await db.Matches
            .AsNoTracking()
            .Where(m =>
                (m.User2Id == user.Id && m.User1.IsActive && !m.User1.IsFirstLogin && m.User1.EmailConfirmed && m.User1.IsBand != null)
                || (m.User1Id == user.Id && m.User2.IsActive && !m.User2.IsFirstLogin && m.User2.EmailConfirmed && m.User2.IsBand != null))
            .OrderBy(m => m.Id)
            .Skip(offset)
            .Take(limit)
            .Select(m => m.User1Id == user.Id ? m.User2Id : m.User1Id)
            .ToListAsync(cancellationToken);

        var unorderedRows = await db.Users
            .AsNoTracking()
            .Where(u => orderedOtherUserIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.IsBand,
                u.Name,
                u.ProfileDescription,
                u.CountryId,
                u.CityId,
                TagsIds = u.Tags
                    .Select(t => t.Id)
                    .ToList(),
                MusicSamples = u.MusicSamples
                    .OrderBy(ms => ms.DisplayOrder)
                    .Select(ms => new { ms.Id, ms.FileName })
                    .ToList(),
                ProfilePictures = u.ProfilePictures
                    .OrderBy(pp => pp.DisplayOrder)
                    .Select(pp => new { pp.Id, pp.FileName })
                    .ToList(),
                BirthDate = u.IsBand == false
                    ? db.Artists
                        .Where(a => a.UserId == u.Id)
                        .Select(a => (DateOnly?)a.BirthDate)
                        .FirstOrDefault()
                    : null,
                BandMembers = u.IsBand == true
                    ? db.Bands
                        .Where(b => b.UserId == u.Id)
                        .SelectMany(b => b.Members.OrderBy(mem => mem.DisplayOrder))
                        .Select(mem => new BandMemberDto(mem.Name, mem.Age, mem.BandRoleId))
                        .ToList()
                    : new List<BandMemberDto>(),
            })
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        // Preserve the match ordering (OrderBy Match.Id) lost by the id-set lookup above.
        var rowsById = unorderedRows.ToDictionary(r => r.Id);
        var rows = orderedOtherUserIds
            .Where(rowsById.ContainsKey)
            .Select(id => rowsById[id])
            .ToList();

        var userProfiles = rows.Select(r => r.IsBand == true
            ? (OtherUserProfileResponse)new OtherUserProfileBandResponse
            {
                Id = r.Id,
                IsBand = r.IsBand,
                Name = r.Name
                    ?? throw new InvalidOperationException($"Active user should NOT have Name = NULL. User id: {r.Id}"),
                ProfileDescription = r.ProfileDescription,
                CountryId = r.CountryId is not null
                    ? (Guid)r.CountryId
                    : throw new InvalidOperationException($"Active user should NOT have CountryId = NULL. User id: {r.Id}"),
                CityId = r.CityId is not null
                    ? (Guid)r.CityId
                    : throw new InvalidOperationException($"Active user should NOT have CityId = NULL. User id: {r.Id}"),
                TagsIds = r.TagsIds,
                MusicSamples = r.MusicSamples
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName, httpRequest)))
                    .ToList(),
                ProfilePictures = r.ProfilePictures
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName, httpRequest)))
                    .ToList(),
                BandMembers = r.BandMembers,
            }
            : new OtherUserProfileArtistResponse
            {
                Id = r.Id,
                IsBand = r.IsBand,
                Name = r.Name
                    ?? throw new InvalidOperationException($"Active user should NOT have Name = NULL. User id: {r.Id}"),
                ProfileDescription = r.ProfileDescription,
                CountryId = r.CountryId is not null
                    ? (Guid)r.CountryId
                    : throw new InvalidOperationException($"Active user should NOT have CountryId = NULL. User id: {r.Id}"),
                CityId = r.CityId is not null
                    ? (Guid)r.CityId
                    : throw new InvalidOperationException($"Active user should NOT have CityId = NULL. User id: {r.Id}"),
                TagsIds = r.TagsIds,
                MusicSamples = r.MusicSamples
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName, httpRequest)))
                    .ToList(),
                ProfilePictures = r.ProfilePictures
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName, httpRequest)))
                    .ToList(),
                BirthDate = r.BirthDate,
            }).ToList();

        return TypedResults.Ok(userProfiles);
    }
}
