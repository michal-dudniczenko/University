namespace Common.Domain.Events;

public abstract class Event
{
    public DateTime Timestamp { get; protected set; } = DateTime.Now;
    public Guid Id { get; set; } = Guid.NewGuid();
}
