namespace Soundmates.Api.Common.Entities;

internal sealed class Message
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Content { get; set; }

    public bool IsSeen { get; set; } = false;

    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public required Guid SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public required Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
}
