namespace Soundmates.Api.Common;

internal static class AppConstants
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 32;

    public const string SamplesDirectoryPath = "samples/";
    public const int MaxSampleSizeMb = 100;
    public const int MaxSampleSize = MaxSampleSizeMb * 1024 * 1024;
    public static readonly string[] AllowedSampleContentTypes = ["AUDIO/MPEG", "VIDEO/MP4"];
    public static readonly string[] AllowedSampleFileExtensions = [".MP3", ".MP4"];
    public const int MaxMusicSamplesCount = 5;

    public const string ImagesDirectoryPath = "images/";
    public const int MaxImageSizeMb = 5;
    public const int MaxImageSize = MaxImageSizeMb * 1024 * 1024;
    public static readonly string[] AllowedImageContentTypes = ["IMAGE/JPEG", "IMAGE/JPG"];
    public static readonly string[] AllowedImageFileExtensions = [".JPEG", ".JPG"];
    public const int MaxProfilePicturesCount = 5;

    public const int MaxUserEmailLength = 100;
    public const int MaxUserNameLength = 50;
    public const int MaxUserDescriptionLength = 500;

    public static readonly DateOnly MinUserBirthDate = new(1900, 1, 1);

    public const int MaxBandMembersCount = 100;

    public const int MaxBandMemberNameLength = 50;
    public const int MinBandMemberAge = 0;
    public const int MaxBandMemberAge = 100;

    public const int MaxMessageContentLength = 4000;

    public const string ClientAppCorsName = "AllowClient5555";
}
