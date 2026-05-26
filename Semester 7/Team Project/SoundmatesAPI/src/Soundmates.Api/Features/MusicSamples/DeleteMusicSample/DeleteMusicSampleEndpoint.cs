using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.MusicSamples.DeleteMusicSample;

internal static class DeleteMusicSampleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteMusicSample(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/music-samples/{musicSampleId}", HandleAsync)
            .WithName("DeleteMusicSample")
            .WithSummary("Delete a music sample")
            .WithDescription("Deletes the specified music sample belonging to the current user.")
            .WithTags("MusicSamples")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string musicSampleId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] ILoggerFactory loggerFactory,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(musicSampleId, fieldName: "musicSampleId");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var musicSampleGuid = Guid.Parse(musicSampleId);

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var sample = await db.MusicSamples
            .AsNoTracking()
            .FirstOrDefaultAsync(ms => ms.Id == musicSampleGuid, cancellationToken);

        if (sample is null)
            return TypedResults.Problem(detail: "No music sample with specified id.", statusCode: 404);

        if (sample.UserId != user.Id)
            return TypedResults.Problem(detail: "You can only delete your own music samples.", statusCode: 401);

        var filePath = Path.Combine("wwwroot", UserMediaUrlHelpers.GetMusicSampleUrl(sample.FileName));
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger(nameof(DeleteMusicSampleEndpoint)).LogError(
                    ex, "Failed to delete music sample file. File path: {FilePath}", filePath);

                return TypedResults.Problem(detail: "Failed to delete music sample file.", statusCode: 500);
            }
        }

        await db.MusicSamples
            .Where(ms => ms.Id == musicSampleGuid)
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
