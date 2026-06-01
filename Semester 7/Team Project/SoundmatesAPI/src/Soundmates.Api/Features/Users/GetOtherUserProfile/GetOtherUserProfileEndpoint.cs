using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Common;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.GetOtherUserProfile;

internal static class GetOtherProfileEndpoint
{
    public static IEndpointRouteBuilder MapGetOtherUserProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId}", HandleAsync)
            .WithName("GetOtherProfile")
            .WithSummary("Get another user's public profile")
            .WithDescription("Returns the public profile of another user (artist or band).")
            .Produces<OtherUserProfileResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Users");

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string userId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(userId, fieldName: "id");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var otherUserGuid = Guid.Parse(userId);

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var otherUser = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == otherUserGuid)
            .Select(u => new { u.IsActive, u.IsFirstLogin, u.EmailConfirmed, u.IsBand })
            .FirstOrDefaultAsync(cancellationToken);

        if (otherUser is null || !otherUser.IsActive || otherUser.IsFirstLogin || !otherUser.EmailConfirmed || otherUser.IsBand is null)
            return TypedResults.Problem(detail: $"No user with ID: {userId}", statusCode: 404);

        if ((bool)otherUser.IsBand)
        {
            var band = await db.Bands
                .AsNoTracking()
                .Include(b => b.Members)
                .Include(b => b.User)
                    .ThenInclude(u => u.Tags)
                .Include(b => b.User)
                    .ThenInclude(u => u.MusicSamples)
                .Include(b => b.User)
                    .ThenInclude(u => u.ProfilePictures)
                .FirstOrDefaultAsync(b => b.UserId == otherUserGuid, cancellationToken);

            if (band is null)
                return TypedResults.Problem(detail: $"Band with userId = {userId} not found.", statusCode: 404);

            return TypedResults.Ok<OtherUserProfileResponse>(new OtherUserProfileBandResponse
            {
                Id = band.User.Id,
                IsBand = band.User.IsBand,
                Name = band.User.Name!,
                ProfileDescription = band.User.ProfileDescription,
                CountryId = (Guid)band.User.CountryId!,
                CityId = (Guid)band.User.CityId!,
                TagsIds = band.User.Tags.Select(t => t.Id).ToList(),
                MusicSamples = band.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName, httpRequest)))
                    .ToList(),
                ProfilePictures = band.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName, httpRequest)))
                    .ToList(),
                BandMembers = band.Members.OrderBy(m => m.DisplayOrder)
                    .Select(bm => new BandMemberDto(bm.Name, bm.Age, bm.BandRoleId)).ToList()
            });
        }
        else
        {
            var artist = await db.Artists
                .AsNoTracking()
                .Include(a => a.User)
                    .ThenInclude(u => u.Tags)
                .Include(a => a.User)
                    .ThenInclude(u => u.MusicSamples)
                .Include(a => a.User)
                    .ThenInclude(u => u.ProfilePictures)
                .FirstOrDefaultAsync(a => a.UserId == otherUserGuid, cancellationToken);

            if (artist is null)
                return TypedResults.Problem(detail: $"Artist with userId = {userId} not found.", statusCode: 404);

            return TypedResults.Ok<OtherUserProfileResponse>(new OtherUserProfileArtistResponse
            {
                Id = artist.User.Id,
                IsBand = artist.User.IsBand,
                Name = artist.User.Name
                    ?? throw new InvalidOperationException($"Active user should NOT have Name = NULL. User id: {artist.User.Id}"),
                ProfileDescription = artist.User.ProfileDescription,
                CountryId = artist.User.CountryId is not null
                    ? (Guid)artist.User.CountryId
                    : throw new InvalidOperationException($"Active user should NOT have CountryId = NULL. User id: {artist.User.Id}"),
                CityId = artist.User.CityId is not null
                    ? (Guid)artist.User.CityId
                    : throw new InvalidOperationException($"Active user should NOT have CityId = NULL. User id: {artist.User.Id}"),
                TagsIds = artist.User.Tags
                    .Select(t => t.Id)
                    .ToList(),
                MusicSamples = artist.User.MusicSamples
                    .OrderBy(ms => ms.DisplayOrder)
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName, httpRequest)))
                    .ToList(),
                ProfilePictures = artist.User.ProfilePictures
                    .OrderBy(pp => pp.DisplayOrder)
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName, httpRequest)))
                    .ToList(),
                BirthDate = artist.BirthDate
            });
        }
    }
}
