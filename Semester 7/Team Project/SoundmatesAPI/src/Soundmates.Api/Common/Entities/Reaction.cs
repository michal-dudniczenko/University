namespace Soundmates.Api.Common.Entities;

internal abstract class Reaction : EntityBase
{
    public required Guid GiverId { get; set; }
    public User Giver { get; set; } = null!;

    public required Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
}
