using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetBandRoles;

internal static class GetBandRolesEndpoint
{
    public static IEndpointRouteBuilder MapGetBandRoles(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/band-roles", HandleAsync)
            .WithName("GetBandRoles")
            .WithSummary("Get all band roles")
            .WithDescription("Returns a list of all available band roles ordered by name.")
            .Produces<List<BandRoleResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var roles = await db.BandRoles
            .AsNoTracking()
            .OrderBy(br => br.Name)
            .Select(br => new BandRoleResponse(br.Id, br.Name))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(roles);
    }
}
