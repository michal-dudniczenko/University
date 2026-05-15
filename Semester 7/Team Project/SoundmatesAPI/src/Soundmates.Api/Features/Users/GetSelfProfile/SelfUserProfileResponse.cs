using Soundmates.Api.Features.Users.Common;
using System.Text.Json.Serialization;

namespace Soundmates.Api.Features.Users.GetSelfProfile;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(SelfUserProfileArtistResponse), "artist")]
[JsonDerivedType(typeof(SelfUserProfileBandResponse), "band")]
internal class SelfUserProfileResponse
{
    public required Guid Id { get; set; }
    public required bool? IsBand { get; set; }
    public required string Email { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
    public required Guid? CountryId { get; set; }
    public required Guid? CityId { get; set; }
    public required bool IsFirstLogin { get; set; }
    public required IList<Guid> TagsIds { get; set; }
    public required IList<MusicSampleDto> MusicSamples { get; set; }
    public required IList<ProfilePictureDto> ProfilePictures { get; set; }
}

internal sealed class SelfUserProfileArtistResponse : SelfUserProfileResponse
{
    public required DateOnly BirthDate { get; set; }
    public required Guid GenderId { get; set; }
}

internal sealed class SelfUserProfileBandResponse : SelfUserProfileResponse
{
    public required IList<BandMemberDto> BandMembers { get; set; }
}
