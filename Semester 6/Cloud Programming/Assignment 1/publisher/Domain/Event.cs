namespace Domain;

public abstract class Event(string from, int id)
{
    public string From { get; set; } = from;
    public int Id { get; set; } = id;

    public override string ToString()
    {
        return $"{GetType().Name} - from: {From} id: {Id}";
    }
}
