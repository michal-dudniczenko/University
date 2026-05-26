using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetTags;

internal static class GetTagsEndpoint
{
    public static IEndpointRouteBuilder MapGetTags(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/tags", HandleAsync)
            .WithName("GetTags")
            .WithSummary("Get all tags")
            .WithDescription("Returns a list of all available tags ordered by name.")
            .Produces<List<TagResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var tags = await db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagResponse(t.Id, t.Name, t.TagCategoryId))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(tags);
    }
}
