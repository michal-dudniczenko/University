using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;

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
        IWebHostEnvironment env,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        if (file is null)
            return TypedResults.Problem(detail: "A file is required.", statusCode: StatusCodes.Status400BadRequest);

        var contentType = file.ContentType ?? string.Empty;
        var extension = Path.GetExtension(file.FileName) ?? string.Empty;

        if (!ApplicationConstants.AllowedSampleContentTypes.TryGetValue(contentType, out var allowedExtensions)
            || !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            var allowedContentTypes = string.Join(", ", ApplicationConstants.AllowedSampleContentTypes
                .Select(kvp => $"{kvp.Key} ({string.Join(", ", kvp.Value)})"));

            return TypedResults.Problem(detail: $"Allowed content types: {allowedContentTypes}.", statusCode: 400);
        }

        if (file.Length > ApplicationConstants.MaximumSampleSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {ApplicationConstants.MaximumSampleSizeMb} MB.", statusCode: 400);

        var currentCount = await db.MusicSamples
            .AsNoTracking()
            .CountAsync(ms => ms.UserId == user.Id, cancellationToken);

        if (currentCount >= ApplicationConstants.MaximumMusicSamplesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {ApplicationConstants.MaximumMusicSamplesCount} music samples.", statusCode: 400);

        var fileName = $"{Guid.CreateVersion7()}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(env.WebRootPath, ApplicationConstants.SamplesDirectoryName, fileName);

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
