namespace SoundmatesAPI.DTOs;

public class UserProfileDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int BirthYear { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required bool IsFirstLogin { get; set; }
}
