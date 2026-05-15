using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Persistence;
using System.Security.Claims;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Features.MusicSamples.UploadMusicSample;

internal static class UploadMusicSampleEndpoint
{
    public static IEndpointRouteBuilder MapUploadMusicSample(this IEndpointRouteBuilder app)
    {
        app.MapPost("/music-samples", HandleAsync)
            .WithName("UploadMusicSample")
            .WithSummary("Upload a music sample")
            .WithDescription("Uploads an MP3 or MP4 audio file as a music sample for the current user's profile.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("MusicSamples")
            .RequireAuthorization()
            .DisableAntiforgery();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromForm] IFormFile file,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var contentType = file.ContentType?.ToUpperInvariant() ?? string.Empty;
        var extension = Path.GetExtension(file.FileName)?.ToUpperInvariant() ?? string.Empty;

        if (!AllowedSampleContentTypes.Contains(contentType) || !AllowedSampleFileExtensions.Contains(extension))
            return TypedResults.Problem(detail: $"Allowed file extensions: {string.Join(", ", AllowedSampleFileExtensions)}", statusCode: 400);

        if (file.Length > MaxSampleSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {MaxSampleSizeMb} MB.", statusCode: 400);

        var currentCount = await db.MusicSamples
            .AsNoTracking()
            .CountAsync(ms => ms.UserId == user.Id, cancellationToken);

        if (currentCount >= MaxMusicSamplesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {MaxMusicSamplesCount} music samples.", statusCode: 400);

        var fileName = $"{Guid.NewGuid()}{extension}";
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
