using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.DTOs;

public class ChangePasswordDto
{
    [Required]
    public required string OldPassword { get; set; }

    [Required]
    [Password]
    public required string NewPassword { get; set; }
}
