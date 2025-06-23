using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Models;

public class MusicSample
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string FileName { get; set; }
    public required Guid UserId { get; set; }
}
