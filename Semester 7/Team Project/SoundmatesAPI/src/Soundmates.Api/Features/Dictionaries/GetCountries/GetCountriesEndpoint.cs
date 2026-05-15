using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetCountries;

internal static class GetCountriesEndpoint
{
    public static IEndpointRouteBuilder MapGetCountries(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/countries", HandleAsync)
            .WithName("GetCountries")
            .WithSummary("Get all countries")
            .WithDescription("Returns a list of all available countries ordered by name.")
            .Produces<List<CountryResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    public static async Task<Ok<List<CountryResponse>>> HandleAsync(
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var countries = await db.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryResponse(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(countries);
    }
}
