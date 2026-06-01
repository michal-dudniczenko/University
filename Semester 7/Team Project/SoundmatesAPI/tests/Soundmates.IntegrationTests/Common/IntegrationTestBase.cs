namespace Soundmates.IntegrationTests.Common;

[Collection(TestConstants.IntegrationTestsCollectionName)]
public abstract class IntegrationTestBase(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    protected CustomWebApplicationFactory Factory { get; } = factory;

    /// <summary>
    /// Default unauthenticated client (https base address, auto-redirect off, unique remote IP).
    /// Use <see cref="CustomWebApplicationFactory.CreateApiClient"/> for additional clients.
    /// </summary>
    protected HttpClient HttpClient { get; private set; } = null!;

    /// <summary>Emails captured by the test email service since the current test started.</summary>
    internal IReadOnlyList<CapturedEmail> SentEmails => Factory.CapturedEmails.SentEmails;

    public async ValueTask InitializeAsync()
    {
        await Factory.ResetStateAsync();
        HttpClient = Factory.CreateApiClient();
    }

    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
