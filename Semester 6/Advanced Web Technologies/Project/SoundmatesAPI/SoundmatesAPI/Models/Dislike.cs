namespace SoundmatesAPI.Models;

public class Dislike
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required Guid GiverId { get; set; }
    public required Guid ReceiverId { get; set; }
}
