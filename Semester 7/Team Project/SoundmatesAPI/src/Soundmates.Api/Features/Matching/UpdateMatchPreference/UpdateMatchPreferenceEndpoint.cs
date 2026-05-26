using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.UpdateMatchPreference;

internal static class UpdateMatchPreferenceEndpoint
{
    public static IEndpointRouteBuilder MapUpdateMatchPreference(this IEndpointRouteBuilder app)
    {
        app.MapPut("/matching/match-preference", HandleAsync)
            .WithName("UpdateMatchPreference")
            .WithSummary("Update match preferences")
            .WithDescription("Updates the authenticated user's matching preference settings.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Matching")
            .AddEndpointFilter<ValidationFilter<UpdateMatchPreferenceRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateMatchPreferenceRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        Guid? countryId = request.CountryId is not null ? Guid.Parse(request.CountryId) : null;
        Guid? cityId = request.CityId is not null ? Guid.Parse(request.CityId) : null;
        Guid? artistGenderId = request.ArtistGenderId is not null ? Guid.Parse(request.ArtistGenderId) : null;

        List<Tag> tags = [];
        if (request.FilterTagsIds is { Count: > 0 })
        {
            var filterTagGuids = request.FilterTagsIds.Select(Guid.Parse).Distinct().ToList();
            tags = await db.Tags
                .Where(t => filterTagGuids.Contains(t.Id))
                .ToListAsync(cancellationToken);

            if (tags.Count != filterTagGuids.Count)
            {
                var invalidTagId = filterTagGuids.First(tagId => tags.All(t => t.Id != tagId));
                throw new InvalidOperationException($"Invalid tag id provided: {invalidTagId}");
            }
        }

        var existing = await db.UserMatchPreferences
            .Include(mp => mp.Tags)
            .FirstOrDefaultAsync(mp => mp.UserId == user.Id, cancellationToken);

        if (existing is null)
        {
            var newPref = new UserMatchPreference
            {
                ShowArtists = request.ShowArtists,
                ShowBands = request.ShowBands,
                MaxDistance = request.MaxDistance,
                CountryId = countryId,
                CityId = cityId,
                ArtistMinAge = request.ArtistMinAge,
                ArtistMaxAge = request.ArtistMaxAge,
                ArtistGenderId = artistGenderId,
                BandMinMembersCount = request.BandMinMembersCount,
                BandMaxMembersCount = request.BandMaxMembersCount,
                UserId = user.Id
            };
            foreach (var tag in tags) newPref.Tags.Add(tag);
            db.UserMatchPreferences.Add(newPref);
        }
        else
        {
            existing.ShowArtists = request.ShowArtists;
            existing.ShowBands = request.ShowBands;
            existing.MaxDistance = request.MaxDistance;
            existing.CountryId = countryId;
            existing.CityId = cityId;
            existing.ArtistMinAge = request.ArtistMinAge;
            existing.ArtistMaxAge = request.ArtistMaxAge;
            existing.ArtistGenderId = artistGenderId;
            existing.BandMinMembersCount = request.BandMinMembersCount;
            existing.BandMaxMembersCount = request.BandMaxMembersCount;

            existing.Tags.Clear();
            foreach (var tag in tags) existing.Tags.Add(tag);
        }

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
