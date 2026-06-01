using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

namespace Soundmates.IntegrationTests.Common.SignalR;

/// <summary>
/// Connects a SignalR client to the in-memory EventHub (via the TestServer handler, long-polling
/// transport, JWT in the <c>access_token</c> query string) and records named events so tests can
/// assert real-time notifications and their payloads.
/// </summary>
internal sealed class EventHubTestClient : IAsyncDisposable
{
    private readonly HubConnection _connection;
    private readonly ConcurrentDictionary<string, Channel<JsonElement>> _channels = new();

    private EventHubTestClient(HubConnection connection) => _connection = connection;

    /// <summary>
    /// Connects and subscribes to <paramref name="eventNames"/>. Register every event you intend to
    /// wait on before the server might send it (i.e. before triggering the action under test).
    /// </summary>
    public static async Task<EventHubTestClient> ConnectAsync(
        CustomWebApplicationFactory factory,
        string accessToken,
        params string[] eventNames)
    {
        // Use https + AccessTokenProvider: the provider puts the JWT in the access_token query string
        // on negotiate AND every poll request (which the API's JwtBearerEvents reads for /eventHub),
        // and https avoids the UseHttpsRedirection 307 that breaks the long-polling handshake.
        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri("https://localhost/eventHub"), options =>
            {
                options.Transports = HttpTransportType.LongPolling;
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
                options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
            })
            .Build();

        var client = new EventHubTestClient(connection);
        foreach (var eventName in eventNames)
        {
            var channel = Channel.CreateUnbounded<JsonElement>();
            client._channels[eventName] = channel;
            connection.On<JsonElement>(eventName, payload => channel.Writer.TryWrite(payload));
        }

        await connection.StartAsync();
        return client;
    }

    /// <summary>Waits for the next occurrence of <paramref name="eventName"/> and returns its payload.</summary>
    public async Task<JsonElement> WaitForEventAsync(string eventName, TimeSpan? timeout = null)
    {
        if (!_channels.TryGetValue(eventName, out var channel))
        {
            throw new InvalidOperationException(
                $"Event '{eventName}' was not registered in ConnectAsync.");
        }

        using var cts = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(5));
        return await channel.Reader.ReadAsync(cts.Token);
    }

    /// <summary>Returns true if no occurrence of <paramref name="eventName"/> arrives within the window.</summary>
    public async Task<bool> NoEventReceivedAsync(string eventName, TimeSpan? window = null)
    {
        if (!_channels.TryGetValue(eventName, out var channel))
        {
            throw new InvalidOperationException(
                $"Event '{eventName}' was not registered in ConnectAsync.");
        }

        using var cts = new CancellationTokenSource(window ?? TimeSpan.FromMilliseconds(750));
        try
        {
            await channel.Reader.ReadAsync(cts.Token);
            return false;
        }
        catch (OperationCanceledException)
        {
            return true;
        }
    }

    public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
}
