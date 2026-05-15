namespace Soundmates.Api.Common.Entities;

internal sealed class Band
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public ICollection<BandMember> Members { get; } = [];

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
