using Soundmates.Api.Common.Constants;

namespace Soundmates.Api.Common.Helpers;

internal static class UserMediaUrlHelpers
{
    public static string GetMusicSampleUrl(string fileName, HttpRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return $"{request.Scheme}://{request.Host}/{ApplicationConstants.SamplesDirectoryName}/{fileName}";
    }

    public static string GetProfilePictureUrl(string fileName, HttpRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return $"{request.Scheme}://{request.Host}/{ApplicationConstants.ImagesDirectoryName}/{fileName}";
    }
}
