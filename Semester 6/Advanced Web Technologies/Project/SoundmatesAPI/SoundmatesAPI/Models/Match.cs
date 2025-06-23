namespace SoundmatesAPI.Models;

public class Match
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required Guid User1Id { get; set; }
    public required Guid User2Id { get; set; }
}
