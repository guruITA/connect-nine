using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Connect.Infrastructure.Health;

// Opens and closes one connection to PostgreSQL. It reads and changes nothing (#311).
// Not Database.CanConnectAsync: that one swallows the error, so the log could not say why.
// Its own connection with Timeout=3, because Npgsql ignores the cancel of the health check while it
// waits for the server to answer, and would otherwise wait its default of 15 seconds.
public sealed class DatabaseHealthCheck(ConnectDbContext dbContext) : IHealthCheck
{
    private const int TimeoutSeconds = 3;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            NpgsqlConnectionStringBuilder connectionString = new(dbContext.Database.GetConnectionString())
            {
                Timeout = TimeoutSeconds,
            };

            await using NpgsqlConnection connection = new(connectionString.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            // HealthCheckService writes the exception to the log. It never goes into the answer.
            return HealthCheckResult.Unhealthy("The database cannot be reached.", exception);
        }
    }
}
