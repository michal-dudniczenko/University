namespace Soundmates.Api.Common.Entities;

internal sealed class Message : EntityBase
{
    public required string Content { get; set; }
    public bool IsSeen { get; set; } = false;

    public required Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public required Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
}
