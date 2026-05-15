namespace Soundmates.Api.Features.Matching.GetMatchPreference;

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
