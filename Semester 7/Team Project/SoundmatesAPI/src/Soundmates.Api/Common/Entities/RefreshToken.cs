namespace Soundmates.Api.Common.Entities;

internal sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string RefreshTokenHash { get; set; }
    public required DateTime RefreshTokenExpiresAt { get; set; }

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
