using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Models;

public class User
{
    public Guid Id { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    public string PasswordHash { get; set; } = "";

    [MaxLength(50)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string Description { get; set; } = "";

    [BirthYear]
    public int BirthYear { get; set; } = -1;

    [MaxLength(100)]
    public string City { get; set; } = "";

    [MaxLength(100)]
    public string Country { get; set; } = "";

    public bool IsActive { get; set; } = false;
}
