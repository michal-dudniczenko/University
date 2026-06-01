namespace Soundmates.IntegrationTests.Messages.Contracts;

// Local mirrors of the API's Messages DTOs. Per project rules the test project never references
// the API's internal DTO records; it mirrors their JSON shape here in this namespace.

/// <summary>Mirrors SendMessageRequest (POST /messages body).</summary>
internal sealed record SendMessageRequest(string ReceiverId, string Content);

/// <summary>
/// Mirrors the API's MessageResponse used by GetConversation and GetConversationsPreview.
/// Note: the API record names the second positional member "Timestamp" but it is serialized as
/// "timestamp" (camelCase) and represents the message's CreatedAt.
/// </summary>
internal sealed record MessageResponse(
    string Content,
    DateTime Timestamp,
    Guid SenderId,
    Guid ReceiverId,
    bool IsSeen);
