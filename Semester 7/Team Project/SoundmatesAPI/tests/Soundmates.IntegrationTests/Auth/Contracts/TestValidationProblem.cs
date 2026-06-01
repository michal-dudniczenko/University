namespace Soundmates.IntegrationTests.Auth.Contracts;

/// <summary>
/// Minimal mirror of ASP.NET Core's ValidationProblemDetails for asserting 422 responses.
/// </summary>
internal sealed record TestValidationProblem
{
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
