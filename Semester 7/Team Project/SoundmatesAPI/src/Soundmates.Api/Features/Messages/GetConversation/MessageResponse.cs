namespace Soundmates.Api.Features.Messages.GetConversation;

internal sealed record MessageResponse(
    string Content,
    DateTime Timestamp,
    Guid SenderId,
    Guid ReceiverId,
    bool IsSeen);
