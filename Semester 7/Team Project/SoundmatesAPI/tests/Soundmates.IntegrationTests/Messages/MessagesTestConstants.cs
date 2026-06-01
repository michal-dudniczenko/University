namespace Soundmates.IntegrationTests.Messages;

/// <summary>
/// Route and reusable domain constants specific to the Messages domain tests
/// (3.29 - 3.32 in tests-plan.md). Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class MessagesTestConstants
{
    public const string SendMessageRoute = "/messages";
    public const string ConversationsPreviewRoute = "/messages/preview";

    /// <summary>Builds GET /messages/{otherUserId} (caller appends ?limit&amp;offset).</summary>
    public static string ConversationRoute(object otherUserId) => $"/messages/{otherUserId}";

    /// <summary>Builds POST /messages/{otherUserId}/view.</summary>
    public static string ViewConversationRoute(object otherUserId) => $"/messages/{otherUserId}/view";

    // SignalR event names emitted by the Messages endpoints.
    public const string MessageReceivedEvent = "MessageReceived";
    public const string ConversationSeenEvent = "ConversationSeen";

    // Default message content used across the suite.
    public const string DefaultContent = "Hello there";

    // SendMessageValidator: Content max length is ApplicationConstants.MaximumMessageContentLength.
    public const int MaxContentLength = 4000;
}
