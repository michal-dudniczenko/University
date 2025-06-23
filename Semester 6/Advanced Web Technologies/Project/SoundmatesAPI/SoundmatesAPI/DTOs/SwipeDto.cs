using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.DTOs;

public class SwipeDto
{
    [Required]
    public Guid GiverId { get; set; }

    [Required]
    public Guid ReceiverId { get; set; }
}
