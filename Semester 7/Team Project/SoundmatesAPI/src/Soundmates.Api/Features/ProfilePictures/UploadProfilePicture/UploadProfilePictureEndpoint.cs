using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Persistence;
using System.Security.Claims;
using static Soundmates.Api.Common.AppConstants;

namespace Soundmates.Api.Features.ProfilePictures.UploadProfilePicture;

internal static class UploadProfilePictureEndpoint
{
    public static IEndpointRouteBuilder MapUploadProfilePicture(this IEndpointRouteBuilder app)
    {
        app.MapPost("/profile-pictures", HandleAsync)
            .WithName("UploadProfilePicture")
            .WithSummary("Upload a profile picture")
            .WithDescription("Uploads a JPEG image as a profile picture for the current user's profile.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("ProfilePictures")
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

        if (!AllowedImageContentTypes.Contains(contentType) || !AllowedImageFileExtensions.Contains(extension))
            return TypedResults.Problem(detail: $"Allowed file extensions: {string.Join(", ", AllowedImageFileExtensions)}", statusCode: 400);

        if (file.Length > MaxImageSize)
            return TypedResults.Problem(detail: $"File size cannot exceed {MaxImageSizeMb} MB.", statusCode: 400);

        var currentCount = await db.ProfilePictures
            .AsNoTracking()
            .CountAsync(pp => pp.UserId == user.Id, cancellationToken);

        if (currentCount >= MaxProfilePicturesCount)
            return TypedResults.Problem(detail: $"User can upload maximum of {MaxProfilePicturesCount} profile pictures.", statusCode: 400);

        var fileName = $"{Guid.NewGuid()}{extension}";
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
