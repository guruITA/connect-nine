using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Connect.Tests.Fixtures;

// Keeps every log line the server writes, with its message, its template values and the values
// of its scopes, so a test can read what would have gone to the log (#312).
public sealed class CapturingLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ConcurrentQueue<CapturedLogLine> _lines = new();
    private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

    public IReadOnlyCollection<CapturedLogLine> Lines => _lines.ToArray();

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, this);

    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

    public void Dispose()
    {
    }

    private sealed class CapturingLogger(string category, CapturingLoggerProvider provider) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => provider._scopeProvider.Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Dictionary<string, string?> scopes = [];
            provider._scopeProvider.ForEachScope(
                (scope, values) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object?>> pairs)
                    {
                        foreach (KeyValuePair<string, object?> pair in pairs)
                        {
                            values[pair.Key] = pair.Value?.ToString();
                        }
                    }
                },
                scopes);

            Dictionary<string, string?> templateValues = [];
            if (state is IEnumerable<KeyValuePair<string, object?>> statePairs)
            {
                foreach (KeyValuePair<string, object?> pair in statePairs)
                {
                    templateValues[pair.Key] = pair.Value?.ToString();
                }
            }

            provider._lines.Enqueue(new CapturedLogLine(
                logLevel,
                category,
                formatter(state, exception),
                templateValues,
                scopes,
                exception));
        }
    }
}
