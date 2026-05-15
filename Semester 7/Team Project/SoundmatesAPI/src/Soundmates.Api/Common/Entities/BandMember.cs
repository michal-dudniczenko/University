namespace Soundmates.Api.Common.Entities;

internal sealed class BandMember
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; }

    public required int Age { get; set; }

    public required int DisplayOrder { get; set; }

    public required Guid BandId { get; set; }
    public Band Band { get; set; } = null!;

    public required Guid BandRoleId { get; set; }
    public BandRole BandRole { get; set; } = null!;
}
