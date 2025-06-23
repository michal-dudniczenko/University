using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Models;

public class Message
{
    public Guid Id { get; set; }

    [MaxLength(4000)]
    public required string Content { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
}
