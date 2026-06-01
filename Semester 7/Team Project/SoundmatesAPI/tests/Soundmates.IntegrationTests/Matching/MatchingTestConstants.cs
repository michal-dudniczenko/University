namespace Soundmates.IntegrationTests.Matching;

/// <summary>
/// Route and reusable domain constants for the Matching domain tests.
/// Do NOT add entries to the shared <c>TestConstants</c>.
/// </summary>
internal static class MatchingTestConstants
{
    public const string LikeRoute = "/matching/like";
    public const string DislikeRoute = "/matching/dislike";
    public const string MatchesRoute = "/matching/matches";
    public const string MatchPreferenceRoute = "/matching/match-preference";
    public const string ArtistsRoute = "/matching/artists";
    public const string BandsRoute = "/matching/bands";
    public const string MatchExistsRoute = "/matching/match/exists";
    public const string UnmatchRoute = "/matching/unmatch";

    // Default pagination used by list-endpoint happy paths.
    public const string DefaultPaging = "?limit=50&offset=0";

    // SignalR event names emitted by CreateLike.
    public const string MatchReceivedEvent = "MatchReceived";
    public const string MatchCreatedEvent = "MatchCreated";

    // GuidValidator always emits the literal key "fieldName" (it does not use the passed
    // field name) — route-GUID 422 bodies on MatchExists/Unmatch are keyed by "fieldName".
    public const string RouteGuidErrorKey = "fieldName";

    // PaginationValidator error keys.
    public const string LimitErrorKey = "Limit";
    public const string OffsetErrorKey = "Offset";
}
