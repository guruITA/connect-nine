using Connect.Presentation.Health.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Health.Endpoints;

public sealed class HealthAnswerWriterTests
{
    [TestCase(HealthStatus.Healthy)]
    [TestCase(HealthStatus.Degraded)]
    public async Task WriteAsync_NotUnhealthy_WritesShortHealthyBody(HealthStatus status)
    {
        DefaultHttpContext context = CreateContext();
        HealthReport report = CreateReport(new HealthReportEntry(
            status, description: null, TimeSpan.FromMilliseconds(12), exception: null, data: null));

        await HealthAnswerWriter.WriteAsync(context, report);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(ReadBody(context), Is.EqualTo("""{"status":"healthy"}"""));
        }
    }

    [Test]
    public async Task WriteAsync_UnhealthyWithSecretInException_LeavesOutEveryDetail()
    {
        DefaultHttpContext context = CreateContext();
        HealthReport report = CreateReport(new HealthReportEntry(
            HealthStatus.Unhealthy,
            description: "The database cannot be reached.",
            TimeSpan.FromMilliseconds(3001),
            exception: new InvalidOperationException("Host=db;Username=connect;Password=secret"),
            data: null));

        await HealthAnswerWriter.WriteAsync(context, report);
        string body = ReadBody(context);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status503ServiceUnavailable));
            Assert.That(body, Does.Contain("\"failing\":[\"database\"]"));
            Assert.That(body, Does.Not.Contain("secret"));
            Assert.That(body, Does.Not.Contain("Password"));
            Assert.That(body, Does.Not.Contain("cannot be reached"));
            Assert.That(body, Does.Not.Contain("3001"));
        }
    }

    [TestCase(HealthStatus.Healthy)]
    [TestCase(HealthStatus.Unhealthy)]
    public async Task WriteAsync_AnyStatus_SetsNoStore(HealthStatus status)
    {
        DefaultHttpContext context = CreateContext();
        HealthReport report = CreateReport(new HealthReportEntry(
            status, description: null, TimeSpan.Zero, exception: null, data: null));

        await HealthAnswerWriter.WriteAsync(context, report);

        Assert.That(context.Response.Headers.CacheControl.ToString(), Is.EqualTo("no-store"));
    }

    private static DefaultHttpContext CreateContext()
    {
        return new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
            Response = { Body = new MemoryStream() },
        };
    }

    private static HealthReport CreateReport(HealthReportEntry databaseEntry)
    {
        return new HealthReport(
            new Dictionary<string, HealthReportEntry> { ["database"] = databaseEntry },
            databaseEntry.Duration);
    }

    private static string ReadBody(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;
        using StreamReader reader = new(context.Response.Body);
        return reader.ReadToEnd();
    }
}
