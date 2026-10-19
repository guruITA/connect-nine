using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Connect.Tests.Health;

public sealed class HealthEndpointTests
{
    // Nothing listens on port 5499, so the real database check of Infrastructure must fail.
    private const string UnreachableDatabase =
        "Host=localhost;Port=5499;Database=connect;Username=connect;Password=secret";

    [Test]
    public async Task GetHealth_DatabaseUnreachable_Returns503NamingDatabaseWithinFiveSeconds()
    {
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", UnreachableDatabase));
        using HttpClient client = factory.CreateClient();
        Stopwatch stopwatch = Stopwatch.StartNew();

        using HttpResponseMessage response = await client.GetAsync("/api/health");
        stopwatch.Stop();
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(body, Does.Contain("\"failing\":[\"database\"]"));
            Assert.That(body, Does.Not.Contain("secret"));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));
        }
    }
}
