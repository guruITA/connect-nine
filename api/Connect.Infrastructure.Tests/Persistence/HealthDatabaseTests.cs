using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Connect.Infrastructure.Health;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class HealthDatabaseTests
{
    [Test]
    public async Task CheckHealthAsync_RealDatabase_ReturnsHealthy()
    {
        await using ConnectDbContext dbContext = CreateDbContext(TestsConnectionString());

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

    [Test]
    public async Task CheckHealthAsync_DatabaseGoesDownAfterHealthyCheck_ReturnsUnhealthy()
    {
        // A relay in front of the real database, so the test can take the database away while the app runs.
        NpgsqlConnectionStringBuilder realDatabase = new(TestsConnectionString());
        string databaseHost = realDatabase.Host
            ?? throw new InvalidOperationException("ConnectionStrings:TestsConnection has no Host.");
        DatabaseRelay relay = new(databaseHost, realDatabase.Port);
        realDatabase.Host = "127.0.0.1";
        realDatabase.Port = relay.Port;
        await using ConnectDbContext dbContext = CreateDbContext(realDatabase.ConnectionString);
        DatabaseHealthCheck check = new(dbContext);

        HealthCheckResult before = await check.CheckHealthAsync(new HealthCheckContext());
        relay.Stop();
        HealthCheckResult after = await check.CheckHealthAsync(new HealthCheckContext());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(before.Status, Is.EqualTo(HealthStatus.Healthy));
            Assert.That(after.Status, Is.EqualTo(HealthStatus.Unhealthy));
        }
    }

    // The same test database as the other database tests, through user secrets or the environment.
    private static string TestsConnectionString() =>
        TestConfiguration.Create().GetConnectionString("TestsConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:TestsConnection is not configured.");

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

    // Passes every byte between the health check and the real database, until Stop closes the
    // listener and every open connection, like a database that went down.
    private sealed class DatabaseRelay
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly List<TcpClient> connections = [];

        public DatabaseRelay(string targetHost, int targetPort)
        {
            listener.Start();
            _ = AcceptAsync(targetHost, targetPort);
        }

        public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;

        public void Stop()
        {
            listener.Stop();
            lock (connections)
            {
                connections.ForEach(connection => connection.Dispose());
            }
        }

        private async Task AcceptAsync(string targetHost, int targetPort)
        {
            try
            {
                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    TcpClient target = new();
                    await target.ConnectAsync(targetHost, targetPort);
                    lock (connections)
                    {
                        connections.Add(client);
                        connections.Add(target);
                    }

                    _ = PumpAsync(client.GetStream(), target.GetStream());
                    _ = PumpAsync(target.GetStream(), client.GetStream());
                }
            }
            catch (Exception exception) when (exception is SocketException or ObjectDisposedException)
            {
                // Stop closed the listener.
            }
        }

        private static async Task PumpAsync(NetworkStream from, NetworkStream to)
        {
            try
            {
                await from.CopyToAsync(to);
            }
            catch (Exception exception) when (exception is IOException or ObjectDisposedException)
            {
                // Stop closed one of the two sides.
            }
        }
    }
}
