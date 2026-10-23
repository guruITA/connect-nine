using System.Net;
using System.Text.Json;
using Connect.Presentation.Health.Endpoints;
using Connect.Presentation.Screen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Screen;

// The screen and the 404 of an unknown /api address next to the real endpoints (#267).
[TestFixture]
public sealed class ScreenFallbackTests
{
    private const string ScreenPage = "<!doctype html><div id=\"root\"></div>";

    private string _webRoot;
    private IHost _host;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        // A web root with only index.html stands in for web/dist, which the pipeline copies into wwwroot.
        _webRoot = Directory.CreateTempSubdirectory("connect-screen-").FullName;
        File.WriteAllText(Path.Combine(_webRoot, "index.html"), ScreenPage);

        _host = new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .UseWebRoot(_webRoot)
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddProblemDetails();
                    services
                        .AddHealthChecks()
                        .AddCheck("database", () => HealthCheckResult.Healthy());
                })
                .Configure(app =>
                {
                    app.UseScreen();
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapHealthEndpoints());
                }))
            .Start();
        _client = _host.GetTestClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _host.Dispose();
        Directory.Delete(_webRoot, recursive: true);
    }

    [TestCase("GET")]
    [TestCase("POST")]
    public async Task UnknownApiAddress_Returns404ProblemDetails(string method)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), "/api/does-not-exist");
        using HttpResponseMessage response = await _client.SendAsync(request);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("This address does not exist"));
        }
    }

    [Test]
    public async Task GetHealth_WithScreenFallback_StillReturnsHealthy()
    {
        using HttpResponseMessage response = await _client.GetAsync("/api/health");
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body, Is.EqualTo("""{"status":"healthy"}"""));
        }
    }

    [Test]
    public async Task PostHealth_WithScreenFallback_StillReturnsMethodNotAllowed()
    {
        using HttpResponseMessage response = await _client.PostAsync("/api/health", content: null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [TestCase("/")]
    [TestCase("/create-account")]
    public async Task GetScreenAddress_ReturnsTheScreen(string address)
    {
        using HttpResponseMessage response = await _client.GetAsync(address);
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/html"));
            Assert.That(body, Is.EqualTo(ScreenPage));
        }
    }

    [Test]
    public async Task GetMissingFile_Returns404()
    {
        using HttpResponseMessage response = await _client.GetAsync("/missing.svg");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
