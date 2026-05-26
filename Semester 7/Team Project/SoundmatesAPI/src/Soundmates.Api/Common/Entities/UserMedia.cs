namespace Soundmates.Api.Common.Entities;

internal abstract class UserMedia : EntityBase
{
    public required string FileName { get; set; }
    public required int DisplayOrder { get; set; }

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
