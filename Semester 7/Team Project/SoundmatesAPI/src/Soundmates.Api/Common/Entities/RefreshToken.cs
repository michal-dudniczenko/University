namespace Soundmates.Api.Common.Entities;

internal sealed class RefreshToken : EntityBase
{
    public required byte[] TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
