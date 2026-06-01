namespace Soundmates.IntegrationTests.ProfilePictures.Contracts;

// Local copies of API response records — NEVER reference src types.
// Kept in the ProfilePictures namespace to avoid collisions.

// The UploadProfilePicture and DeleteProfilePicture endpoints both return TypedResults.Ok()
// (no body), so there are no dedicated response DTOs.

/// <summary>
/// Minimal mirror of ASP.NET Core's ValidationProblemDetails for asserting 422 responses.
/// </summary>
internal sealed record TestValidationProblem
{
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
