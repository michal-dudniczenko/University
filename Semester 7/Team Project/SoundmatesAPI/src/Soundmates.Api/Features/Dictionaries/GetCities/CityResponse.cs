namespace Soundmates.Api.Features.Dictionaries.GetCities;

internal sealed record CityResponse(Guid Id, string Name, double Latitude, double Longitude, Guid CountryId);
