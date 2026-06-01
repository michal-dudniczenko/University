using System.Text.Json.Serialization;

namespace Soundmates.IntegrationTests.Users.Contracts;

// Local copies of the API request/response shapes — NEVER reference src types.
// Kept in the Users namespace (NOT Common / NOT Matching) so the identically-named
// profile DTOs in the Matching domain do not collide.

// ---------------------------------------------------------------------------
// Shared media / member DTOs (mirror Users.Common.* shapes)
// ---------------------------------------------------------------------------

internal sealed record MusicSampleDto(Guid Id, string FileUrl);

internal sealed record ProfilePictureDto(Guid Id, string FileUrl);

internal sealed record BandMemberDto(string Name, int Age, Guid BandRoleId);

// ---------------------------------------------------------------------------
// GetSelfProfile response shapes (polymorphic on "userType"; base is returned
// directly for a first-login user whose IsBand is still null, in which case the
// payload carries no discriminator — the base type is concrete here).
// Only the self profile includes Email.
// ---------------------------------------------------------------------------

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(SelfUserProfileArtistResponse), "artist")]
[JsonDerivedType(typeof(SelfUserProfileBandResponse), "band")]
internal class GetSelfUserProfileResponse
{
    public Guid Id { get; set; }
    public bool? IsBand { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public string? ProfileDescription { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? CityId { get; set; }
    public bool IsFirstLogin { get; set; }
    public IList<Guid> TagsIds { get; set; } = [];
    public IList<MusicSampleDto> MusicSamples { get; set; } = [];
    public IList<ProfilePictureDto> ProfilePictures { get; set; } = [];
}

internal sealed class SelfUserProfileArtistResponse : GetSelfUserProfileResponse
{
    public DateOnly BirthDate { get; set; }
    public Guid GenderId { get; set; }
}

internal sealed class SelfUserProfileBandResponse : GetSelfUserProfileResponse
{
    public IList<BandMemberDto> BandMembers { get; set; } = [];
}

// ---------------------------------------------------------------------------
// GetOtherUserProfile response shapes (polymorphic on "userType"; no Email).
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

// ---------------------------------------------------------------------------
// UpdateProfile request bodies. Polymorphic on the "userType" discriminator so
// System.Text.Json serializes the discriminator first, matching the src request.
// ---------------------------------------------------------------------------

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(UpdateUserProfileArtistRequest), "artist")]
[JsonDerivedType(typeof(UpdateUserProfileBandRequest), "band")]
internal abstract class UpdateUserProfileRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public string CityId { get; set; } = null!;
    public IList<string> TagsIds { get; set; } = [];
    public IList<string> MusicSamplesOrder { get; set; } = [];
    public IList<string> ProfilePicturesOrder { get; set; } = [];
}

internal sealed class UpdateUserProfileArtistRequest : UpdateUserProfileRequest
{
    public string BirthDate { get; set; } = null!;
    public string GenderId { get; set; } = null!;
}

internal sealed class UpdateUserProfileBandRequest : UpdateUserProfileRequest
{
    public IList<BandMemberRequestDto> BandMembers { get; set; } = [];
}

internal sealed record BandMemberRequestDto(string Name, int Age, string BandRoleId);

// ---------------------------------------------------------------------------
// Minimal mirror of ASP.NET Core's ValidationProblemDetails / ProblemDetails.
// ---------------------------------------------------------------------------

internal sealed record TestValidationProblem
{
    public Dictionary<string, string[]> Errors { get; init; } = new();
}

internal sealed record TestProblemDetails
{
    public string? Detail { get; init; }
    public int? Status { get; init; }
    public string? Title { get; init; }
    public string? TraceId { get; init; }
}
