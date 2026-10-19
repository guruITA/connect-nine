using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Connect.Infrastructure.Health;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class HealthDatabaseTests
{
    [Test]
    public async Task CheckHealthAsync_RealDatabase_ReturnsHealthy()
    {
        string connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not configured.");
        await using ConnectDbContext dbContext = CreateDbContext(connectionString);

        HealthCheckResult result = await new DatabaseHealthCheck(dbContext)
            .CheckHealthAsync(new HealthCheckContext());

        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public async Task CheckHealthAsync_NothingListens_ReturnsUnhealthy()
    {
        await using ConnectDbContext dbContext = CreateDbContext(ConnectionStringFor(port: 5499));

        HealthCheckResult result = await new DatabaseHealthCheck(dbContext)
            .CheckHealthAsync(new HealthCheckContext());

        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task CheckHealthAsync_DatabaseNeverAnswers_ReturnsUnhealthyWithinFourSeconds()
    {
        // A server that takes the connection and never answers, like a database that hangs.
        using TcpListener silentServer = new(IPAddress.Loopback, 0);
        silentServer.Start();
        int port = ((IPEndPoint)silentServer.LocalEndpoint).Port;
        await using ConnectDbContext dbContext = CreateDbContext(ConnectionStringFor(port));
        Stopwatch stopwatch = Stopwatch.StartNew();

        HealthCheckResult result = await new DatabaseHealthCheck(dbContext)
            .CheckHealthAsync(new HealthCheckContext());
        stopwatch.Stop();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(4)));
        }
    }

    private static string ConnectionStringFor(int port)
    {
        return $"Host=127.0.0.1;Port={port};Database=connect;Username=connect;Password=secret";
    }

    private static ConnectDbContext CreateDbContext(string connectionString)
    {
        DbContextOptions<ConnectDbContext> options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ConnectDbContext(options);
    }
}
