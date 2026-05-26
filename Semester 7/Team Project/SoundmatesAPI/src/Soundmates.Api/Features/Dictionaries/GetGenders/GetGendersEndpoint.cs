using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetGenders;

internal static class GetGendersEndpoint
{
    public static IEndpointRouteBuilder MapGetGenders(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/genders", HandleAsync)
            .WithName("GetGenders")
            .WithSummary("Get all genders")
            .WithDescription("Returns a list of all available genders ordered by name.")
            .Produces<List<GenderResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var genders = await db.Genders
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new GenderResponse(g.Id, g.Name))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(genders);
    }
}
