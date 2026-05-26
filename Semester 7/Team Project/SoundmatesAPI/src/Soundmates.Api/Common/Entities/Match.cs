namespace Soundmates.Api.Common.Entities;

internal sealed class Match : EntityBase
{
    public required Guid User1Id { get; set; }
    public User User1 { get; set; } = null!;

    public required Guid User2Id { get; set; }
    public User User2 { get; set; } = null!;
}
