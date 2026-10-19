using System.Net;
using System.Text.Json;
using Connect.Presentation.Health.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Health.Endpoints;

public sealed class HealthEndpointTests
{
    [Test]
    public async Task GetHealth_DatabaseHealthy_ReturnsHealthyWithoutLogin()
    {
        using IHost host = StartHost(HealthStatus.Healthy);
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health");
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body, Is.EqualTo("""{"status":"healthy"}"""));
            Assert.That(response.Headers.CacheControl?.NoStore, Is.True);
        }
    }

    [Test]
    public async Task GetHealth_DatabaseUnhealthy_Returns503NamingDatabase()
    {
        using IHost host = StartHost(HealthStatus.Unhealthy);
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health");
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("Connect is not healthy"));
            Assert.That(body.RootElement.GetProperty("failing")[0].GetString(), Is.EqualTo("database"));
            Assert.That(response.Headers.CacheControl?.NoStore, Is.True);
        }
    }

    [Test]
    public async Task PostHealth_ReturnsMethodNotAllowed()
    {
        using IHost host = StartHost(HealthStatus.Healthy);
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.PostAsync("/api/health", content: null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    // The real database check lives in Infrastructure, which this test project does not use.
    // A check with a fixed result stands in for it.
    private static IHost StartHost(HealthStatus databaseStatus)
    {
        return new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services
                        .AddHealthChecks()
                        .AddCheck("database", () => new HealthCheckResult(databaseStatus));
                })
                .Configure(app => app.UseRouting().UseEndpoints(endpoints => endpoints.MapHealthEndpoints())))
            .Start();
    }
}
