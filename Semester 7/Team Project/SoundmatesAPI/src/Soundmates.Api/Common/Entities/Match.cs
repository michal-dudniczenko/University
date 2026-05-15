namespace Soundmates.Api.Common.Entities;

internal sealed class Match
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public required Guid User1Id { get; set; }
    public User User1 { get; set; } = null!;

    public required Guid User2Id { get; set; }
    public User User2 { get; set; } = null!;
}
