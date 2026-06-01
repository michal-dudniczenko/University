using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Soundmates.IntegrationTests.Common.Infrastructure;

// TEMP DIAGNOSTIC: captures server-side log messages (incl. unhandled-exception logs from the
// global exception handler) so tests can read why an endpoint returned 500.
internal sealed class InMemoryLogSink
{
    public ConcurrentQueue<string> Entries { get; } = new();

    public void Clear() => Entries.Clear();
}

internal sealed class InMemoryLoggerProvider(InMemoryLogSink sink) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new InMemoryLogger(categoryName, sink);

    public void Dispose() { }

    private sealed class InMemoryLogger(string category, InMemoryLogSink sink) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel < LogLevel.Warning)
            {
                return;
            }

            sink.Entries.Enqueue($"[{logLevel}] {category}: {formatter(state, exception)}{(exception is not null ? "\n" + exception : string.Empty)}");
        }
    }
}
