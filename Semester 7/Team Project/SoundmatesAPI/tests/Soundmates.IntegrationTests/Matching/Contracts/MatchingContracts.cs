using System.Text.Json.Serialization;

namespace Soundmates.IntegrationTests.Matching.Contracts;

// Local copies of the API request/response records — NEVER reference src types.
// Kept in the Matching namespace to avoid collisions with identically-named records
// in the Users domain (which mirror the same polymorphic profile shapes).

// ---------------------------------------------------------------------------
// Request bodies
// ---------------------------------------------------------------------------

/// <summary>
/// Minimal mirror of ASP.NET Core's ValidationProblemDetails for asserting 422 responses.
/// Kept in the Matching namespace to avoid colliding with the Auth domain's copy.
/// </summary>
internal sealed record TestValidationProblem
{
    public Dictionary<string, string[]> Errors { get; init; } = new();
}

internal sealed record CreateLikeRequest(string ReceiverId);

internal sealed record CreateDislikeRequest(string ReceiverId);

internal sealed record UpdateMatchPreferenceRequest(
    bool ShowArtists,
    bool ShowBands,
    int? MaxDistance,
    string? CountryId,
    string? CityId,
    int? ArtistMinAge,
    int? ArtistMaxAge,
    string? ArtistGenderId,
    int? BandMinMembersCount,
    int? BandMaxMembersCount,
    IList<string> FilterTagsIds);

// ---------------------------------------------------------------------------
// GetMatchPreference response
// ---------------------------------------------------------------------------

internal sealed record MatchPreferenceResponse(
    bool ShowArtists,
    bool ShowBands,
    int? MaxDistance,
    Guid? CountryId,
    Guid? CityId,
    int? ArtistMinAge,
    int? ArtistMaxAge,
    Guid? ArtistGenderId,
    int? BandMinMembersCount,
    int? BandMaxMembersCount,
    IList<Guid> FilterTagsIds);

// ---------------------------------------------------------------------------
// Shared media / member DTOs (mirror Users.Common.* shapes)
// ---------------------------------------------------------------------------

internal sealed record MusicSampleDto(Guid Id, string FileUrl);

internal sealed record ProfilePictureDto(Guid Id, string FileUrl);

internal sealed record BandMemberDto(string Name, int Age, Guid BandRoleId);

// ---------------------------------------------------------------------------
// Polymorphic user-profile response shapes used by GetMatches /
// GetPotentialMatchesArtists / GetPotentialMatchesBands.
// Discriminator property name "userType" mirrors GetOtherUserProfileResponse.
// ---------------------------------------------------------------------------

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(OtherUserProfileArtistResponse), "artist")]
[JsonDerivedType(typeof(OtherUserProfileBandResponse), "band")]
internal abstract class GetOtherUserProfileResponse
{
    public Guid Id { get; set; }
    public bool? IsBand { get; set; }
    public string Name { get; set; } = null!;
    public string? ProfileDescription { get; set; }
    public Guid CountryId { get; set; }
    public Guid CityId { get; set; }
    public IList<Guid> TagsIds { get; set; } = [];
    public IList<MusicSampleDto> MusicSamples { get; set; } = [];
    public IList<ProfilePictureDto> ProfilePictures { get; set; } = [];
}

internal sealed class OtherUserProfileArtistResponse : GetOtherUserProfileResponse
{
    public DateOnly? BirthDate { get; set; }
}

internal sealed class OtherUserProfileBandResponse : GetOtherUserProfileResponse
{
    public IList<BandMemberDto> BandMembers { get; set; } = [];
}
