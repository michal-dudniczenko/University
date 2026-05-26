namespace Soundmates.Api.Common.Entities;

internal sealed class City : EntityBase
{
    public required string Name { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }

    public required Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
