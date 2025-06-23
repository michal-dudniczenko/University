using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.DTOs;

public class SendMessageDto
{
    [Required]
    public required Guid ReceiverId { get; set; }

    [MaxLength(4000)]
    [Required]
    public required string Content { get; set; }
}
