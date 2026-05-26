namespace Soundmates.Api.Common.Entities;

internal sealed class PendingRegistration : EntityBase
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required byte[] EmailTokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
