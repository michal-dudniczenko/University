namespace SoundmatesAPI.DTOs;

using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [Password]
    public required string Password { get; set; }
}
