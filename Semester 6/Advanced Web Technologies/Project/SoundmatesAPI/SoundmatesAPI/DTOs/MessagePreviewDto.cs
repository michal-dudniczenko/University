namespace SoundmatesAPI.DTOs;

public class MessagePreviewDto
{
    public required string Content { get; set; }
    public required DateTime Timestamp { get; set; }
    public required string UserName { get; set; }
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
}

