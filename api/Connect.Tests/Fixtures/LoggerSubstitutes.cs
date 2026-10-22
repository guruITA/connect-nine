using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;

namespace Connect.Tests.Fixtures;

// A substitute ILoggerProvider that gives every category its own substitute ILogger, so a log test of
// #312 reads the lines the whole server writes from ReceivedCalls() (codeConventions.md, Tests and
// mocking, rule 4).
public sealed class LoggerSubstitutes
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    // ReceivedCalls() keeps the arguments of a call, not the scopes that were open at that moment, so
    // the substitute logger reads them while it is called. The key is the state of that call.
    private readonly ConditionalWeakTable<object, IReadOnlyDictionary<string, string?>> _scopes = new();

    public LoggerSubstitutes()
    {
        // ISupportExternalScope, as the console logger has, so the logger factory hands over the scopes
        // of a request, such as the one with the RequestId.
        Provider = Substitute.For<ILoggerProvider, ISupportExternalScope>();
        Provider.CreateLogger(Arg.Any<string>())
            .Returns(call => _loggers.GetOrAdd(call.Arg<string>(), _ => NewLogger()));
    }

    public ILoggerProvider Provider { get; }

    public IReadOnlyCollection<LoggedLine> Lines =>
        _loggers
            .SelectMany(pair => pair.Value.ReceivedCalls()
                .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
                .Select(call => ToLine(pair.Key, call.GetArguments())))
            .ToArray();

    private ILogger NewLogger()
    {
        ILogger logger = Substitute.For<ILogger>();
        logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        logger
            .When(substitute => substitute.Log(
                Arg.Any<LogLevel>(),
                Arg.Any<EventId>(),
                Arg.Any<Arg.AnyType>(),
                Arg.Any<Exception?>(),
                Arg.Any<Func<Arg.AnyType, Exception?, string>>()))
            .Do(call =>
            {
                if (call.Args()[2] is { } state)
                {
                    _scopes.AddOrUpdate(state, OpenScopes());
                }
            });
        return logger;
    }

    // The values of every scope that is open right now, from the scope provider of the logger factory.
    private Dictionary<string, string?> OpenScopes()
    {
        Dictionary<string, string?> values = [];
        ICall? handover = Provider.ReceivedCalls()
            .LastOrDefault(call => call.GetMethodInfo().Name == nameof(ISupportExternalScope.SetScopeProvider));
        if (handover?.GetArguments()[0] is IExternalScopeProvider scopeProvider)
        {
            scopeProvider.ForEachScope(
                (scope, found) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object?>> pairs)
                    {
                        foreach (KeyValuePair<string, object?> pair in pairs)
                        {
                            found[pair.Key] = pair.Value?.ToString();
                        }
                    }
                },
                values);
        }

        return values;
    }

    // The arguments of ILogger.Log: level, event id, state, exception and the formatter of the message.
    private LoggedLine ToLine(string category, object?[] arguments)
    {
        object? state = arguments[2];
        Exception? exception = arguments[3] as Exception;

        Dictionary<string, string?> templateValues = [];
        if (state is IEnumerable<KeyValuePair<string, object?>> pairs)
        {
            foreach (KeyValuePair<string, object?> pair in pairs)
            {
                templateValues[pair.Key] = pair.Value?.ToString();
            }
        }

        IReadOnlyDictionary<string, string?> scopes = new Dictionary<string, string?>();
        if (state is not null && _scopes.TryGetValue(state, out IReadOnlyDictionary<string, string?>? found))
        {
            scopes = found;
        }

        return new LoggedLine(
            arguments[0] is LogLevel logLevel ? logLevel : LogLevel.None,
            category,
            (arguments[4] as Delegate)?.DynamicInvoke(state, exception)?.ToString() ?? string.Empty,
            templateValues,
            scopes,
            exception);
    }
}
