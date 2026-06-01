namespace Soundmates.IntegrationTests.Users;

/// <summary>
/// Route and reusable domain constants for the Users domain tests.
/// Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class UsersTestConstants
{
    public const string SelfProfileRoute = "/users/profile";

    // GetOtherUserProfile is "/users/{userId}". Build via OtherProfileRoute(id).
    public static string OtherProfileRoute(string userId) => $"/users/{userId}";
    public static string OtherProfileRoute(Guid userId) => $"/users/{userId}";

    // UpdateProfile shares the self-profile route but uses PUT.
    public const string UpdateProfileRoute = "/users/profile";

    // The polymorphic discriminator property name used by both request and response bodies.
    public const string UserTypeDiscriminator = "userType";
    public const string ArtistDiscriminator = "artist";
    public const string BandDiscriminator = "band";

    // GuidValidator.ValidateGuid always emits the literal key "fieldName" in the errors
    // dictionary (it ignores the passed field name). The test plan labels GetOtherUserProfile's
    // route-GUID 422 as key "id", but the real emitted key is "fieldName".
    public const string RouteGuidErrorKey = "fieldName";

    // Validation error keys (FluentValidation property names) for UpdateProfile.
    public const string NameKey = "Name";
    public const string DescriptionKey = "Description";
    public const string CountryIdKey = "CountryId";
    public const string CityIdKey = "CityId";
    public const string TagsIdsKey = "TagsIds";
    public const string MusicSamplesOrderKey = "MusicSamplesOrder";
    public const string ProfilePicturesOrderKey = "ProfilePicturesOrder";
    public const string BirthDateKey = "BirthDate";
    public const string GenderIdKey = "GenderId";
    public const string BandMembersKey = "BandMembers";

    // Media url directory prefixes (relative urls built by UserMediaUrlHelpers).
    public const string SamplesUrlPrefix = "samples/";
    public const string ImagesUrlPrefix = "images/";

    // Boundary values mirrored from ApplicationConstants.
    public const int MaxNameLength = 50;
    public const int MaxDescriptionLength = 500;
    public const int MaxBandMemberNameLength = 50;
    public const int MinBandMemberAge = 0;
    public const int MaxBandMemberAge = 100;
    public const int MaximumBandMembersCount = 100; // Count must be < this (99 passes, 100 fails).

    public const string MinBirthDate = "1900-01-01";
    public const string NotAGuid = "not-a-guid";

    // A well-formed GUID that does not correspond to any seeded row.
    public static string NonexistentGuid() => Guid.NewGuid().ToString();
}
