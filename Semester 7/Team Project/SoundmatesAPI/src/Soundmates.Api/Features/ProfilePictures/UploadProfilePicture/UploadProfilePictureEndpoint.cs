using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using System.Security.Claims;
using static Soundmates.Api.Common.Constants.ApplicationConstants;

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
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var contentType = file.ContentType ?? string.Empty;
        var extension = Path.GetExtension(file.FileName) ?? string.Empty;

        if (!AllowedImageContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase)
            || !AllowedImageFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return TypedResults.Problem(detail: $"Allowed file extensions: {string.Join(", ", AllowedImageFileExtensions)}", statusCode: 400);
        }

        if (file.Length > MaximumImageSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {MaximumImageSizeMb} MB.", statusCode: 400);

        var currentCount = await db.ProfilePictures
            .AsNoTracking()
            .CountAsync(pp => pp.UserId == user.Id, cancellationToken);

        if (currentCount >= MaximumProfilePicturesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {MaximumProfilePicturesCount} profile pictures.", statusCode: 400);

        var fileName = $"{Guid.CreateVersion7()}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine("wwwroot", UserMediaUrlHelpers.GetProfilePictureUrl(fileName));

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
