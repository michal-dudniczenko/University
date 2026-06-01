namespace Soundmates.IntegrationTests.MusicSamples;

/// <summary>
/// Route and miscellaneous constants specific to the MusicSamples domain tests.
/// Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class MusicSamplesTestConstants
{
    public const string UploadRoute = "/music-samples";
    public const string DeleteRouteTemplate = "/music-samples/{0}";

    // Disallowed values used for negative file-type tests.
    public const string DisallowedContentType = "image/jpeg";
    public const string DisallowedExtensionFileName = "sample.txt";

    // Individually-valid but cross-format mismatched pairs (F3).
    public const string Mp3FileName = "sample.mp3";
    public const string Mp4FileName = "sample.mp4";

    // Non-GUID string for route GUID validation.
    public const string NonGuidId = "not-a-guid";

    // Maximum allowed music samples per user.
    public const int MaxSamplesCount = 5;

    // Small sample size to use when the exact byte count does not matter.
    public const int SmallFileSizeBytes = 1024;
}
