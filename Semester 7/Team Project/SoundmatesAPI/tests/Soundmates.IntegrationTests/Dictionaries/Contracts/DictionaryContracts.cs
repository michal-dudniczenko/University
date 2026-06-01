namespace Soundmates.IntegrationTests.Dictionaries.Contracts;

// Local copies of the API response records — NEVER reference src types.
// Kept in the Dictionaries namespace to avoid collisions with identically-named
// records that may appear in other test domains.

internal sealed record CountryResponse(Guid Id, string Name);

internal sealed record CityResponse(Guid Id, string Name, double Latitude, double Longitude, Guid CountryId);

internal sealed record GenderResponse(Guid Id, string Name);

internal sealed record TagResponse(Guid Id, string Name, Guid TagCategoryId);

internal sealed record TagCategoryResponse(Guid Id, string Name, bool IsForBand);

internal sealed record BandRoleResponse(Guid Id, string Name);
