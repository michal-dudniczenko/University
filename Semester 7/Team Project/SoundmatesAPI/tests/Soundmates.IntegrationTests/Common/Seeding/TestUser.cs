namespace Soundmates.IntegrationTests.Common.Seeding;

/// <summary>A seeded user plus the plaintext password needed to authenticate as them.</summary>
internal sealed record TestUser(Guid Id, string Email, string Password)
{
    public bool? IsBand { get; init; }
}
