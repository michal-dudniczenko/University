using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetCities;

internal static class GetCitiesEndpoint
{
    public static IEndpointRouteBuilder MapGetCities(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/cities", HandleAsync)
            .WithName("GetCities")
            .WithSummary("Get cities by country")
            .WithDescription("Returns a list of cities for the specified country, ordered by name.")
            .Produces<List<CityResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    public static async Task<Ok<List<CityResponse>>> HandleAsync(
        [FromQuery] Guid countryId,
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var cities = await db.Cities
            .AsNoTracking()
            .Where(c => c.CountryId == countryId)
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse(c.Id, c.Name, c.Latitude, c.Longitude, c.CountryId))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(cities);
    }
}
