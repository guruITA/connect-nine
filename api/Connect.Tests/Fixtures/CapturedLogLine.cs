using Microsoft.Extensions.Logging;

namespace Connect.Tests.Fixtures;

// One log line as CapturingLoggerProvider kept it (#312).
public sealed record CapturedLogLine(
    LogLevel LogLevel,
    string Category,
    string Message,
    IReadOnlyDictionary<string, string?> TemplateValues,
    IReadOnlyDictionary<string, string?> Scopes,
    Exception? Exception)
{
    // Everything of the line that would reach the log as text, to search it for a leak.
    public string AllText =>
        string.Join(
            '\n',
            new[] { Category, Message, Exception?.ToString() }
                .Concat(TemplateValues.Select(pair => $"{pair.Key}={pair.Value}"))
                .Concat(Scopes.Select(pair => $"{pair.Key}={pair.Value}")));
}
