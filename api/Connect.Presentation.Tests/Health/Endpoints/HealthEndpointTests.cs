using System.Net;
using Connect.Presentation.Health.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Health.Endpoints;

public sealed class HealthEndpointTests : IDisposable
{
    private readonly IHost host;

    public HealthEndpointTests()
    {
        host = new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .ConfigureServices(services => services.AddRouting())
                .Configure(app => app.UseRouting().UseEndpoints(endpoints => endpoints.MapHealthEndpoints())))
            .Start();
    }

    [Test]
    public async Task GetHealth_ReturnsHealthyWithoutLogin()
    {
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health");
        string body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(body, Is.EqualTo("""{"status":"healthy"}"""));
    }

    public void Dispose()
    {
        host.Dispose();
    }
}
