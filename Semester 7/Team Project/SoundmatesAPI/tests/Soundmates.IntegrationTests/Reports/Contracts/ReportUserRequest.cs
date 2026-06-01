namespace Soundmates.IntegrationTests.Reports.Contracts;

// Local mirrors of the API's Reports DTOs and shared test helpers.
// Per project rules the test project NEVER references src types;
// these files duplicate only the JSON shape.

internal sealed record ReportUserRequest(string ReportedUserId, string Reason, string Description);

/// <summary>
/// Minimal mirror of ASP.NET Core's ValidationProblemDetails for asserting 422 responses.
/// Local copy so the Reports namespace is self-contained.
/// </summary>
internal sealed record TestValidationProblem
{
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
