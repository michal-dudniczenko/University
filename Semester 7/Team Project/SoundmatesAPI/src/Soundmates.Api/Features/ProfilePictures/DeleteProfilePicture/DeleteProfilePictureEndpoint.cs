using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Helpers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.ProfilePictures.DeleteProfilePicture;

internal static class DeleteProfilePictureEndpoint
{
    public static IEndpointRouteBuilder MapDeleteProfilePicture(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/profile-pictures/{profilePictureId}", HandleAsync)
            .WithName("DeleteProfilePicture")
            .WithSummary("Delete a profile picture")
            .WithDescription("Deletes the specified profile picture belonging to the current user.")
            .WithTags("ProfilePictures")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string profilePictureId,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        [FromServices] ILoggerFactory loggerFactory,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var errors = GuidValidator.ValidateGuid(profilePictureId, fieldName: "profilePictureId");
        if (errors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(errors));

        var profilePictureGuid = Guid.Parse(profilePictureId);

        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var picture = await db.ProfilePictures
            .AsNoTracking()
            .FirstOrDefaultAsync(pp => pp.Id == profilePictureGuid, cancellationToken);

        if (picture is null)
            return TypedResults.Problem(detail: "No profile picture with specified id.", statusCode: 404);

        if (picture.UserId != user.Id)
            return TypedResults.Problem(detail: "You can only delete your own profile pictures.", statusCode: 401);

        var filePath = Path.Combine("wwwroot", UserMediaUrlHelpers.GetProfilePictureUrl(picture.FileName));
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger(nameof(DeleteProfilePictureEndpoint)).LogError(
                    ex, "Failed to delete profile picture file. File path: {FilePath}", filePath);

                return TypedResults.Problem(detail: "Failed to delete profile picture file.", statusCode: 500);
            }
        }

        await db.ProfilePictures
            .Where(pp => pp.Id == profilePictureGuid)
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
