using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Matching.GetMatchPreference;

internal static class GetMatchPreferenceEndpoint
{
    public static IEndpointRouteBuilder MapGetMatchPreference(this IEndpointRouteBuilder app)
    {
        app.MapGet("/matching/match-preference", HandleAsync)
            .WithName("GetMatchPreference")
            .WithSummary("Get the authenticated user's match preferences")
            .WithDescription("Returns the current match preference settings for the authenticated user.")
            .Produces<MatchPreferenceResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Matching")
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var matchPreference = await db.UserMatchPreferences
            .AsNoTracking()
            .Include(mp => mp.Tags)
            .FirstOrDefaultAsync(mp => mp.UserId == user.Id, cancellationToken);

        if (matchPreference is null)
            return TypedResults.Problem(detail: $"Could not get match preference for user with id: {user.Id}", statusCode: 500);

        return TypedResults.Ok(new MatchPreferenceResponse(
            matchPreference.ShowArtists,
            matchPreference.ShowBands,
            matchPreference.MaxDistance,
            matchPreference.CountryId,
            matchPreference.CityId,
            matchPreference.ArtistMinAge,
            matchPreference.ArtistMaxAge,
            matchPreference.ArtistGenderId,
            matchPreference.BandMinMembersCount,
            matchPreference.BandMaxMembersCount,
            matchPreference.Tags.Select(t => t.Id).ToList()));
    }
}
