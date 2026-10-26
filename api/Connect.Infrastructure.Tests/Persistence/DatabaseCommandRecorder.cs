using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Connect.Infrastructure.Tests.Persistence;

// Keeps the SQL that EF Core sends, so a test can check which columns or tables a query reads.
// For tests only, never registered in the server. Public because the whole-server tests of
// Connect.Tests use it too (#347).
public sealed class DatabaseCommandRecorder : DbCommandInterceptor
{
    private readonly ConcurrentQueue<string> commands = new();

    // A trailing space or line break after the table name keeps a hit on "profile" from also
    // matching a table whose name starts the same way, such as "profile_tag".
    public IReadOnlyList<string> CommandsFor(string table)
    {
        return commands.Where(command => command.Contains($"FROM {table} ", StringComparison.Ordinal)
            || command.Contains($"FROM {table}\n", StringComparison.Ordinal)).ToList();
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        commands.Enqueue(command.CommandText);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        commands.Enqueue(command.CommandText);
        return ValueTask.FromResult(result);
    }
}
