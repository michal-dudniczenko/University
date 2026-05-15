using System.Text.Json.Serialization;

namespace Soundmates.Api.Features.Users.UpdateProfile;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(UpdateUserProfileArtistRequest), "artist")]
[JsonDerivedType(typeof(UpdateUserProfileBandRequest), "band")]
internal abstract class UpdateUserProfileRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string CountryId { get; set; }
    public required string CityId { get; set; }
    public required IList<string> TagsIds { get; set; }
    public required IList<string> MusicSamplesOrder { get; set; }
    public required IList<string> ProfilePicturesOrder { get; set; }
}

internal sealed class UpdateUserProfileArtistRequest : UpdateUserProfileRequest
{
    public required string BirthDate { get; set; }
    public required string GenderId { get; set; }
}

internal sealed class UpdateUserProfileBandRequest : UpdateUserProfileRequest
{
    public required IList<BandMemberRequestDto> BandMembers { get; set; }
}

internal sealed record BandMemberRequestDto(string Name, int Age, string BandRoleId);
