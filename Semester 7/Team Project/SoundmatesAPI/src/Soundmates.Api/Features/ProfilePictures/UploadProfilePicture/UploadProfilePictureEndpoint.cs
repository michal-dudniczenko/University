using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.ProfilePictures.UploadProfilePicture;

internal static class UploadProfilePictureEndpoint
{
    public static IEndpointRouteBuilder MapUploadProfilePicture(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profile-pictures", HandleAsync)
            .WithName("UploadProfilePicture")
            .WithSummary("Upload a profile picture")
            .WithDescription("Uploads a JPEG image as a profile picture for the current user's profile.")
            .WithTags("ProfilePictures")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .DisableAntiforgery();

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

        if (!ApplicationConstants.AllowedImageContentTypes.TryGetValue(contentType, out var allowedExtensions)
            || !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            var allowedContentTypes = string.Join(", ", ApplicationConstants.AllowedImageContentTypes
                .Select(kvp => $"{kvp.Key} ({string.Join(", ", kvp.Value)})"));

            return TypedResults.Problem(detail: $"Allowed content types: {allowedContentTypes}.", statusCode: 400);
        }

        if (file.Length > ApplicationConstants.MaximumImageSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {ApplicationConstants.MaximumImageSizeMb} MB.", statusCode: 400);

        var currentCount = await db.ProfilePictures
            .AsNoTracking()
            .CountAsync(pp => pp.UserId == user.Id, cancellationToken);

        if (currentCount >= ApplicationConstants.MaximumProfilePicturesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {ApplicationConstants.MaximumProfilePicturesCount} profile pictures.", statusCode: 400);

        var fileName = $"{Guid.CreateVersion7()}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(env.WebRootPath, ApplicationConstants.ImagesDirectoryName, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        db.ProfilePictures.Add(new ProfilePicture
        {
            FileName = fileName,
            DisplayOrder = currentCount,
            UserId = user.Id
        });

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
