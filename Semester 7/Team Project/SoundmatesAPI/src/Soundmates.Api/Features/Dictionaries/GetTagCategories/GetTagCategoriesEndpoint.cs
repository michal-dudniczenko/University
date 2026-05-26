using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Dictionaries.GetTagCategories;

internal static class GetTagCategoriesEndpoint
{
    public static IEndpointRouteBuilder MapGetTagCategories(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dictionaries/tag-categories", HandleAsync)
            .WithName("GetTagCategories")
            .WithSummary("Get all tag categories")
            .WithDescription("Returns a list of all available tag categories ordered by name.")
            .Produces<List<TagCategoryResponse>>(StatusCodes.Status200OK)
            .WithTags("Dictionaries")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var categories = await db.TagCategories
            .AsNoTracking()
            .OrderBy(tc => tc.Name)
            .Select(tc => new TagCategoryResponse(tc.Id, tc.Name, tc.IsForBand))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(categories);
    }
}
