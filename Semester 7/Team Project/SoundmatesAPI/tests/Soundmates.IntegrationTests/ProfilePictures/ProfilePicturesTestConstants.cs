namespace Soundmates.IntegrationTests.ProfilePictures;

/// <summary>
/// Route and reusable domain constants for the ProfilePictures domain tests.
/// Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class ProfilePicturesTestConstants
{
    public const string UploadRoute = "/profile-pictures";
    public const string DeleteRouteBase = "/profile-pictures";

    // Maximum image size in bytes: 5 MB (mirrors ApplicationConstants.MaximumImageSize).
    public const int MaxImageSizeBytes = 5 * 1024 * 1024;

    // Boundary sizes.
    public const int ExactMaxSizeBytes = MaxImageSizeBytes;       // exactly 5 MB → 200 (handler uses >)
    public const int OversizeBytes = MaxImageSizeBytes + 1;       // 5 MB + 1 byte → 400 (or 413)

    // Maximum count of profile pictures per user (mirrors ApplicationConstants.MaximumProfilePicturesCount).
    public const int MaxPictureCount = 5;

    // GuidValidator always emits the literal key "fieldName" regardless of the fieldName argument.
    public const string RouteGuidErrorKey = "fieldName";

    // Disallowed content-type with an allowed extension — tests F1.
    public const string DisallowedContentType = "image/png";

    // Allowed content-type with a disallowed extension — tests F2.
    public const string DisallowedExtension = ".png";
}
