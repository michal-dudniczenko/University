namespace SoundmatesAPI.DTOs;

public class MessageDto
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required DateTime Timestamp { get; set; }
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
}
