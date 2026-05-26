using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;
using static Soundmates.Api.Common.Constants.ApplicationConstants;

namespace Soundmates.Api.Features.MusicSamples.UploadMusicSample;

internal static class UploadMusicSampleEndpoint
{
    public static IEndpointRouteBuilder MapUploadMusicSample(this IEndpointRouteBuilder app)
    {
        app.MapPost("/music-samples", HandleAsync)
            .WithName("UploadMusicSample")
            .WithSummary("Upload a music sample")
            .WithDescription("Uploads an MP3 or MP4 audio file as a music sample for the current user's profile.")
            .WithTags("MusicSamples")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .DisableAntiforgery(); // needed for JWT

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] IFormFile file,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var contentType = file.ContentType ?? string.Empty;
        var extension = Path.GetExtension(file.FileName) ?? string.Empty;

        if (!AllowedSampleContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase)
            || !AllowedSampleFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return TypedResults.Problem(detail: $"Allowed file extensions: {string.Join(", ", AllowedSampleFileExtensions)}", statusCode: 400);
        }

        if (file.Length > MaximumSampleSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {MaximumSampleSizeMb} MB.", statusCode: 400);

        var currentCount = await db.MusicSamples
            .AsNoTracking()
            .CountAsync(ms => ms.UserId == user.Id, cancellationToken);

        if (currentCount >= MaximumMusicSamplesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {MaximumMusicSamplesCount} music samples.", statusCode: 400);

        var fileName = $"{Guid.CreateVersion7()}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine("wwwroot", UserMediaUrlHelpers.GetMusicSampleUrl(fileName));

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        db.MusicSamples.Add(new MusicSample
        {
            FileName = fileName,
            DisplayOrder = currentCount,
            UserId = user.Id
        });

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
