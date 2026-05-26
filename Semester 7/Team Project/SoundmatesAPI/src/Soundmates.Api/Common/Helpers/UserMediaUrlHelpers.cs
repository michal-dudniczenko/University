using static Soundmates.Api.Common.Constants.ApplicationConstants;

namespace Soundmates.Api.Common.Helpers;

internal static class UserMediaUrlHelpers
{
    public static string GetMusicSampleUrl(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return SamplesDirectoryPath + fileName;
    }

    public static string GetProfilePictureUrl(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return ImagesDirectoryPath + fileName;
    }
}
