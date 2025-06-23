using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.DTOs;

public class RefreshTokenDto
{
    [Required]
    public required string Token { get; set; }
}
