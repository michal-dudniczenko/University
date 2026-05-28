namespace Soundmates.IntegrationTests.Common;

[Collection(TestConstants.IntegrationTestsCollectionName)]
public abstract class IntegrationTestBase(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    protected CustomWebApplicationFactory Factory { get; } = factory;

    protected HttpClient HttpClient { get; private set; } = null!;

    public ValueTask InitializeAsync()
    {
        HttpClient = Factory.CreateClient();
        return Factory.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
