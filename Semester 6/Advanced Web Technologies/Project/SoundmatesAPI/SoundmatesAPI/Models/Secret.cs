using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Models;

public class Secret
{
    [Key]
    public required string SecretKey { get; set; }
}