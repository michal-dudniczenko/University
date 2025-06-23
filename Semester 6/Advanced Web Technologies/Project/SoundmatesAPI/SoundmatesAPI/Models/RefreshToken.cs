using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Models;

public class RefreshToken
{
    [Key]
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpirationDate { get; set; }
    
}
