using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Connect.Tests.Fixtures;

// The test database of the tests that start the whole server (backend rule 20): the connection
// string of ConnectionStrings:TestsConnection, read from the user secrets of api/Connect and the
// environment, as TestConfiguration does in Connect.Infrastructure.Tests.
internal static class TestDatabase
{
    public static string ConnectionString()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets<ApplicationAssemblyMarker>()
            .AddEnvironmentVariables()
            .Build()
            .GetConnectionString("TestsConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:TestsConnection is not configured.");
    }

    public static ConnectDbContext CreateDbContext()
    {
        return new ConnectDbContext(new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(ConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options);
    }
}
