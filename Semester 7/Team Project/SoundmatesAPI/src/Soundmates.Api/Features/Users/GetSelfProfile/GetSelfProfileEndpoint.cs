using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Features.Users.Common;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.GetSelfProfile;

internal static class GetSelfProfileEndpoint
{
    public static IEndpointRouteBuilder MapGetSelfProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/profile", HandleAsync)
            .WithName("GetSelfProfile")
            .WithSummary("Get the authenticated user's full profile")
            .WithDescription("Returns the full profile of the authenticated user (artist or band).")
            .WithTags("Users")
            .Produces<GetSelfUserProfileResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal, checkForFirstLogin: false);
        if (user is null)
            return TypedResults.Unauthorized();

        if (user.IsBand is null)
        {
            return TypedResults.Ok(new GetSelfUserProfileResponse
            {
                Id = user.Id,
                IsBand = user.IsBand,
                Email = user.Email,
                Name = user.Name,
                ProfileDescription = user.ProfileDescription,
                CountryId = user.CountryId,
                CityId = user.CityId,
                IsFirstLogin = user.IsFirstLogin,
                TagsIds = [],
                MusicSamples = [],
                ProfilePictures = []
            });
        }

        if ((bool)user.IsBand)
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
                .FirstOrDefaultAsync(b => b.UserId == user.Id, cancellationToken);

            if (band is null)
                return TypedResults.Problem(detail: $"Band with userId = {user.Id} not found.", statusCode: 404);

            return TypedResults.Ok<GetSelfUserProfileResponse>(new SelfUserProfileBandResponse
            {
                Id = band.User.Id,
                IsBand = band.User.IsBand,
                Email = band.User.Email,
                Name = band.User.Name,
                ProfileDescription = band.User.ProfileDescription,
                CountryId = band.User.CountryId,
                CityId = band.User.CityId,
                IsFirstLogin = band.User.IsFirstLogin,
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
                .FirstOrDefaultAsync(a => a.UserId == user.Id, cancellationToken);

            if (artist is null)
                return TypedResults.Problem(detail: $"Artist with userId = {user.Id} not found.", statusCode: 404);

            return TypedResults.Ok<GetSelfUserProfileResponse>(new SelfUserProfileArtistResponse
            {
                Id = artist.User.Id,
                IsBand = artist.User.IsBand,
                Email = artist.User.Email,
                Name = artist.User.Name,
                ProfileDescription = artist.User.ProfileDescription,
                CountryId = artist.User.CountryId,
                CityId = artist.User.CityId,
                IsFirstLogin = artist.User.IsFirstLogin,
                TagsIds = artist.User.Tags.Select(t => t.Id).ToList(),
                MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder)
                    .Select(ms => new MusicSampleDto(ms.Id, UserMediaUrlHelpers.GetMusicSampleUrl(ms.FileName))).ToList(),
                ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder)
                    .Select(pp => new ProfilePictureDto(pp.Id, UserMediaUrlHelpers.GetProfilePictureUrl(pp.FileName))).ToList(),
                BirthDate = artist.BirthDate,
                GenderId = artist.GenderId
            });
        }
    }
}
