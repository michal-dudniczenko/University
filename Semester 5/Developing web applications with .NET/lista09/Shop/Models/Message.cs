namespace Shop.Models;

public class Message {
    public string Info { get; }
    public bool IsFailure { get; }

    public Message(string info, bool isFailure) {
        Info = info;
        IsFailure = isFailure;
    }
}