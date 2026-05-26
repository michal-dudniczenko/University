namespace Soundmates.Api.Common.Constants;

internal static class ApplicationConstants
{
    public const string DefaultDbCollation_CI_AS_SC = "Latin1_General_100_CI_AS_SC";

    public const double EarthRadiusKm = 6371.0;

    public const string SamplesDirectoryPath = "samples/";
    public const int MaximumSampleSizeMb = 100;
    public const int MaximumSampleSize = MaximumSampleSizeMb * 1024 * 1024;
    public static readonly string[] AllowedSampleContentTypes = ["audio/mpeg", "video/mp4"];
    public static readonly string[] AllowedSampleFileExtensions = [".mp3", ".mp4"];
    public const int MaximumMusicSamplesCount = 5;

    public const string ImagesDirectoryPath = "images/";
    public const int MaximumImageSizeMb = 5;
    public const int MaximumImageSize = MaximumImageSizeMb * 1024 * 1024;
    public static readonly string[] AllowedImageContentTypes = ["image/jpeg", "image/jpg"];
    public static readonly string[] AllowedImageFileExtensions = [".jpeg", ".jpg"];
    public const int MaximumProfilePicturesCount = 5;

    public const int MaximumUserEmailLength = 100;
    public const int MaximumUserNameLength = 50;
    public const int MaximumUserDescriptionLength = 500;
    public static readonly DateOnly MinimumUserBirthDate = new(1900, 1, 1);

    public const int MaximumBandMembersCount = 100;
    public const int MaximumBandMemberNameLength = 50;
    public const int MinimumBandMemberAge = 0;
    public const int MaximumBandMemberAge = 100;

    public const int MaximumMessageContentLength = 4000;
}
