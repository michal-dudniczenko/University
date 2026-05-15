using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.GetOtherProfile;

internal static class GetOtherProfileEndpoint
{
    public static IEndpointRouteBuilder MapGetOtherProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId}", HandleAsync)
            .WithName("GetOtherProfile")
            .WithSummary("Get another user's public profile")
            .WithDescription("Returns the public profile of another user (artist or band).")
            .Produces<OtherUserProfileResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Users")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string userId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(userId, fieldName: "id");
        if (errors is not null)
        {
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));
        }

        var userGuid = Guid.Parse(userId);

        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var otherUser = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userGuid, cancellationToken);

        if (otherUser is null || !otherUser.IsActive || otherUser.IsFirstLogin || !otherUser.IsEmailConfirmed || otherUser.IsBand is null)
            return TypedResults.Problem(detail: $"No user with ID: {userId}", statusCode: 404);

        if ((bool)otherUser.IsBand)
        {
            var band = await db.Bands
                .AsNoTracking()
                .Include(b => b.Members)
                .Include(b => b.User).ThenInclude(u => u.Tags)
                .Include(b => b.User).ThenInclude(u => u.MusicSamples)
                .Include(b => b.User).ThenInclude(u => u.ProfilePictures)
                .FirstOrDefaultAsync(b => b.UserId == userGuid, cancellationToken);

            if (band is null)
                return TypedResults.Problem(detail: $"Band with userId = {userId} not found.", statusCode: 404);

            return TypedResults.Ok<OtherUserProfileResponse>(new OtherUserProfileBandResponse
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
                .FirstOrDefaultAsync(a => a.UserId == userGuid, cancellationToken);

            if (artist is null)
                return TypedResults.Problem(detail: $"Artist with userId = {userId} not found.", statusCode: 404);

            return TypedResults.Ok<OtherUserProfileResponse>(new OtherUserProfileArtistResponse
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
}
