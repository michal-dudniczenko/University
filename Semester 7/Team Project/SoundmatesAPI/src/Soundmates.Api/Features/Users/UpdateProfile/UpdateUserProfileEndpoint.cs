using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
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
            .Accepts<UpdateUserProfileRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Users")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] UpdateUserProfileRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        [FromServices] IServiceProvider serviceProvider,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: false, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        if (request is UpdateUserProfileArtistRequest artistRequest)
        {
            var validator = serviceProvider.GetRequiredService<IValidator<UpdateUserProfileArtistRequest>>();
            var validationResult = await validator.ValidateAsync(artistRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));
            }

            return await HandleArtistUpdateAsync(artistRequest, user.Id, db, cancellationToken);
        }

        if (request is UpdateUserProfileBandRequest bandRequest)
        {
            var validator = serviceProvider.GetRequiredService<IValidator<UpdateUserProfileBandRequest>>();
            var validationResult = await validator.ValidateAsync(bandRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));
            }

            return await HandleBandUpdateAsync(bandRequest, user.Id, db, cancellationToken);
        }

        return TypedResults.Problem(detail: "Invalid userType discriminator.", statusCode: 400);
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

        var artistTags = await db.Tags
            .Include(t => t.TagCategory)
            .Where(t => !t.TagCategory.IsForBand)
            .ToListAsync(cancellationToken);

        existingUser.Tags.Clear();
        foreach (var tagIdStr in request.TagsIds)
        {
            var tagId = Guid.Parse(tagIdStr);
            var tag = artistTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");
            existingUser.Tags.Add(tag);
        }

        await ApplyMediaOrderAsync(db, existingUser, request.MusicSamplesOrder, request.ProfilePicturesOrder, cancellationToken);

        existingUser.IsBand = false;
        existingUser.IsFirstLogin = false;
        existingUser.Name = request.Name.Trim();
        existingUser.Description = request.Description.Trim();
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

        var bandTags = await db.Tags
            .Include(t => t.TagCategory)
            .Where(t => t.TagCategory.IsForBand)
            .ToListAsync(cancellationToken);

        existingUser.Tags.Clear();
        foreach (var tagIdStr in request.TagsIds)
        {
            var tagId = Guid.Parse(tagIdStr);
            var tag = bandTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");
            existingUser.Tags.Add(tag);
        }

        await ApplyMediaOrderAsync(db, existingUser, request.MusicSamplesOrder, request.ProfilePicturesOrder, cancellationToken);

        existingUser.IsBand = true;
        existingUser.IsFirstLogin = false;
        existingUser.Name = request.Name.Trim();
        existingUser.Description = request.Description.Trim();
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

    private static async Task ApplyMediaOrderAsync(
        ApplicationDbContext db,
        User existingUser,
        IList<string> musicSamplesOrder,
        IList<string> profilePicturesOrder,
        CancellationToken cancellationToken)
    {
        var musicSampleGuids = musicSamplesOrder.Select(Guid.Parse).ToList();
        if (musicSampleGuids.Count != musicSampleGuids.Distinct().Count())
            throw new InvalidOperationException("Provided list of music samples contained duplicates.");

        var existingMusicSamples = await db.MusicSamples
            .Where(ms => ms.UserId == existingUser.Id)
            .ToListAsync(cancellationToken);

        existingUser.MusicSamples.Clear();
        int displayOrder = 0;
        foreach (var sampleId in musicSampleGuids)
        {
            var sample = existingMusicSamples.FirstOrDefault(ms => ms.Id == sampleId)
                ?? throw new InvalidOperationException($"Not existing music sample provided with id: {sampleId}");
            sample.DisplayOrder = displayOrder++;
            existingUser.MusicSamples.Add(sample);
        }

        var profilePictureGuids = profilePicturesOrder.Select(Guid.Parse).ToList();
        if (profilePictureGuids.Count != profilePictureGuids.Distinct().Count())
            throw new InvalidOperationException("Provided list of profile pictures contained duplicates.");

        var existingProfilePictures = await db.ProfilePictures
            .Where(pp => pp.UserId == existingUser.Id)
            .ToListAsync(cancellationToken);

        existingUser.ProfilePictures.Clear();
        displayOrder = 0;
        foreach (var pictureId in profilePictureGuids)
        {
            var picture = existingProfilePictures.FirstOrDefault(pp => pp.Id == pictureId)
                ?? throw new InvalidOperationException($"Not existing profile picture provided with id: {pictureId}");
            picture.DisplayOrder = displayOrder++;
            existingUser.ProfilePictures.Add(picture);
        }
    }
}
