using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests;

// The setup every database test shares (backend rule 20): a ConnectDbContext on the database of
// ConnectionStrings:TestsConnection, and the PostgreSQL error behind a write the database refused.
internal static class TestDatabase
{
    public static string ConnectionString()
    {
        return TestConfiguration.Create().GetConnectionString("TestsConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:TestsConnection is not configured.");
    }

    public static ConnectDbContext CreateDbContext(params IInterceptor[] interceptors)
    {
        DbContextOptionsBuilder<ConnectDbContext> builder = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(ConnectionString())
            .UseSnakeCaseNamingConvention();
        if (interceptors.Length > 0)
        {
            builder.AddInterceptors(interceptors);
        }

        return new ConnectDbContext(builder.Options);
    }

    public static PostgresException FindPostgresException(DbUpdateException? exception)
    {
        return exception?.InnerException as PostgresException
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
    }
}
