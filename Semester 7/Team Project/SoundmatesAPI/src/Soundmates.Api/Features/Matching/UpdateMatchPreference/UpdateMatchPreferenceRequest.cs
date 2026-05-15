namespace Soundmates.Api.Features.Matching.UpdateMatchPreference;

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
