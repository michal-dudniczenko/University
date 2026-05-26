using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Globalization;
using System.Security.Claims;

namespace Soundmates.Api.Features.Users.UpdateProfile;

internal static class UpdateUserProfileEndpoint
{
    public static IEndpointRouteBuilder MapUpdateProfile(this IEndpointRouteBuilder app)
    {
        app.MapPut("/users/profile", HandleAsync)
            .WithName("UpdateUserProfile")
            .WithSummary("Update the authenticated user's profile")
            .WithDescription("Updates artist or band profile (polymorphic on userType field). Flips IsFirstLogin to false on first call.")
            .WithTags("Users")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateUserProfileRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] IServiceProvider serviceProvider,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal, checkForFirstLogin: false);
        if (user is null)
            return TypedResults.Unauthorized();

        if (request is UpdateUserProfileArtistRequest artistRequest)
        {
            var problem = await ValidateAsync(serviceProvider, artistRequest, cancellationToken);
            if (problem is not null)
                return TypedResults.UnprocessableEntity(problem);

            return await HandleArtistUpdateAsync(artistRequest, user.Id, db, cancellationToken);
        }

        if (request is UpdateUserProfileBandRequest bandRequest)
        {
            var problem = await ValidateAsync(serviceProvider, bandRequest, cancellationToken);
            if (problem is not null)
                return TypedResults.UnprocessableEntity(problem);

            return await HandleBandUpdateAsync(bandRequest, user.Id, db, cancellationToken);
        }

        return TypedResults.Problem(detail: "Invalid userType discriminator.", statusCode: 400);
    }

    private static async Task<ValidationProblemDetails?> ValidateAsync<T>(
        IServiceProvider serviceProvider,
        T request,
        CancellationToken cancellationToken) where T : class
    {
        var validator = serviceProvider.GetRequiredService<IValidator<T>>();
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (result.IsValid)
            return null;

        return new ValidationProblemDetails(result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
    }

    private static async Task<IResult> HandleArtistUpdateAsync(
        UpdateUserProfileArtistRequest request,
        Guid userId,
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var existingUser = await db.Users
            .Include(u => u.Tags)
            .Include(u => u.ProfilePictures)
            .Include(u => u.MusicSamples)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"User with id {userId} was not found.");

        var requestedTagGuids = request.TagsIds
            .Select(Guid.Parse)
            .ToList();
        var artistTags = await db.Tags
            .Where(t => requestedTagGuids.Contains(t.Id) && !t.TagCategory.IsForBand)
            .ToListAsync(cancellationToken);

        existingUser.Tags.Clear();
        foreach (var tagId in requestedTagGuids)
        {
            var tag = artistTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");
            existingUser.Tags.Add(tag);
        }

        ApplyMediaOrder(existingUser, request.MusicSamplesOrder, request.ProfilePicturesOrder);

        existingUser.IsBand = false;
        existingUser.IsFirstLogin = false;
        existingUser.Name = request.Name.Trim();
        existingUser.ProfileDescription = request.Description.Trim();
        existingUser.CountryId = Guid.Parse(request.CountryId);
        existingUser.CityId = Guid.Parse(request.CityId);

        var birthDate = DateOnly.ParseExact(request.BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var existingArtist = await db.Artists.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        if (existingArtist is null)
        {
            db.Artists.Add(new Artist
            {
                BirthDate = birthDate,
                GenderId = Guid.Parse(request.GenderId),
                UserId = userId,
                User = existingUser
            });
        }
        else
        {
            existingArtist.BirthDate = birthDate;
            existingArtist.GenderId = Guid.Parse(request.GenderId);
        }

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<IResult> HandleBandUpdateAsync(
        UpdateUserProfileBandRequest request,
        Guid userId,
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var existingUser = await db.Users
            .Include(u => u.Tags)
            .Include(u => u.MusicSamples)
            .Include(u => u.ProfilePictures)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"User with id {userId} was not found.");

        var requestedTagGuids = request.TagsIds.Select(Guid.Parse).ToList();
        var bandTags = await db.Tags
            .Where(t => requestedTagGuids.Contains(t.Id) && t.TagCategory.IsForBand)
            .ToListAsync(cancellationToken);

        existingUser.Tags.Clear();
        foreach (var tagId in requestedTagGuids)
        {
            var tag = bandTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");
            existingUser.Tags.Add(tag);
        }

        ApplyMediaOrder(existingUser, request.MusicSamplesOrder, request.ProfilePicturesOrder);

        existingUser.IsBand = true;
        existingUser.IsFirstLogin = false;
        existingUser.Name = request.Name.Trim();
        existingUser.ProfileDescription = request.Description.Trim();
        existingUser.CountryId = Guid.Parse(request.CountryId);
        existingUser.CityId = Guid.Parse(request.CityId);

        var existingBand = await db.Bands
            .Include(b => b.Members)
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);

        if (existingBand is null)
        {
            var band = new Band { UserId = userId, User = existingUser };
            int order = 0;
            foreach (var member in request.BandMembers)
            {
                band.Members.Add(new BandMember
                {
                    Name = member.Name.Trim(),
                    Age = member.Age,
                    DisplayOrder = order++,
                    BandId = band.Id,
                    BandRoleId = Guid.Parse(member.BandRoleId)
                });
            }
            db.Bands.Add(band);
        }
        else
        {
            existingBand.Members.Clear();
            int order = 0;
            foreach (var member in request.BandMembers)
            {
                db.BandMembers.Add(new BandMember
                {
                    Name = member.Name.Trim(),
                    Age = member.Age,
                    DisplayOrder = order++,
                    BandRoleId = Guid.Parse(member.BandRoleId),
                    BandId = existingBand.Id
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }

    // Operates on the already-loaded, tracked navigation collections (loaded via Include on the
    // user query), so no extra round trips are needed to re-read the user's media.
    private static void ApplyMediaOrder(
        User existingUser,
        IList<string> musicSamplesOrder,
        IList<string> profilePicturesOrder)
    {
        var musicSampleGuids = musicSamplesOrder.Select(Guid.Parse).ToList();
        if (musicSampleGuids.Count != musicSampleGuids.Distinct().Count())
            throw new InvalidOperationException("Provided list of music samples contained duplicates.");

        var loadedMusicSamples = existingUser.MusicSamples.ToList();
        existingUser.MusicSamples.Clear();
        int displayOrder = 0;
        foreach (var sampleId in musicSampleGuids)
        {
            var sample = loadedMusicSamples.FirstOrDefault(ms => ms.Id == sampleId)
                ?? throw new InvalidOperationException($"Not existing music sample provided with id: {sampleId}");
            sample.DisplayOrder = displayOrder++;
            existingUser.MusicSamples.Add(sample);
        }

        var profilePictureGuids = profilePicturesOrder.Select(Guid.Parse).ToList();
        if (profilePictureGuids.Count != profilePictureGuids.Distinct().Count())
            throw new InvalidOperationException("Provided list of profile pictures contained duplicates.");

        var loadedProfilePictures = existingUser.ProfilePictures.ToList();
        existingUser.ProfilePictures.Clear();
        displayOrder = 0;
        foreach (var pictureId in profilePictureGuids)
        {
            var picture = loadedProfilePictures.FirstOrDefault(pp => pp.Id == pictureId)
                ?? throw new InvalidOperationException($"Not existing profile picture provided with id: {pictureId}");
            picture.DisplayOrder = displayOrder++;
            existingUser.ProfilePictures.Add(picture);
        }
    }
}
