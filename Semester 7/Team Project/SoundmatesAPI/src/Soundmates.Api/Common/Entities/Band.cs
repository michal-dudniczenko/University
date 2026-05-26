namespace Soundmates.Api.Common.Entities;

internal sealed class Band : EntityBase
{
    public ICollection<BandMember> Members { get; } = [];

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
