using Soundmates.Api.Features.Users.Common;
using System.Text.Json.Serialization;

namespace Soundmates.Api.Features.Users.GetOtherUserProfile;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(OtherUserProfileArtistResponse), "artist")]
[JsonDerivedType(typeof(OtherUserProfileBandResponse), "band")]
internal abstract class GetOtherUserProfileResponse
{
    public required Guid Id { get; set; }
    public required bool? IsBand { get; set; }
    public required string Name { get; set; }
    public required string? ProfileDescription { get; set; }
    public required Guid CountryId { get; set; }
    public required Guid CityId { get; set; }
    public required IList<Guid> TagsIds { get; set; }
    public required IList<MusicSampleDto> MusicSamples { get; set; }
    public required IList<ProfilePictureDto> ProfilePictures { get; set; }
}

internal sealed class OtherUserProfileArtistResponse : GetOtherUserProfileResponse
{
    public required DateOnly? BirthDate { get; set; }
}

internal sealed class OtherUserProfileBandResponse : GetOtherUserProfileResponse
{
    public required IList<BandMemberDto> BandMembers { get; set; }
}
