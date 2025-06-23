using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.DTOs;

public class UpdateUserDto
{
    [Required]
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Description { get; set; }

    [Required]
    [BirthYear]
    public required int BirthYear { get; set; }

    [Required]
    [MaxLength(100)]
    public required string City { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Country { get; set; }
}
