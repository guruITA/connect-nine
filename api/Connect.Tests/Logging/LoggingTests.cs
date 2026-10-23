using Connect.Infrastructure.Persistence;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Connect.Tests.Logging;

// The rules of the log of #312, checked on the lines the whole server writes.
[TestFixture]
public sealed class LoggingTests
{
    // Nothing listens on port 5499, so the database check of /api/health fails without a database.
    private const string UnreachableDatabase =
        "Host=localhost;Port=5499;Database=connect;Username=connect;Password=secret";

    private const string RequestLineCategory = "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware";

    private LoggerSubstitutes _logs;
    private WebApplicationFactory<Program> _factory;

    [SetUp]
    public void SetUp()
    {
        _logs = new LoggerSubstitutes();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", UnreachableDatabase)
                .ConfigureTestServices(services => services.AddSingleton(_logs.Provider)));
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }

    [Test]
    public void Logging_ServerStarted_WritesJsonWithScopesAndUtcTimestamp()
    {
        IServiceProvider services = _factory.Services;

        ConsoleLoggerOptions console = services.GetRequiredService<IOptions<ConsoleLoggerOptions>>().Value;
        JsonConsoleFormatterOptions json = services.GetRequiredService<IOptions<JsonConsoleFormatterOptions>>().Value;
        string[] providers = services.GetServices<ILoggerProvider>()
            .Where(provider => provider != _logs.Provider)
            .Select(provider => provider.GetType().Name)
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(console.FormatterName, Is.EqualTo(ConsoleFormatterNames.Json));
            Assert.That(json.IncludeScopes, Is.True);
            Assert.That(json.UseUtcTimestamp, Is.True);
            Assert.That(json.TimestampFormat, Is.Not.Null.And.EndsWith("Z"));
            Assert.That(providers, Is.EqualTo(new[] { nameof(ConsoleLoggerProvider) }));
        }
    }

    [Test]
    public void DbContext_ServerStarted_NeverLogsSensitiveData()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();

        CoreOptionsExtension? core = dbContext.GetService<IDbContextOptions>().FindExtension<CoreOptionsExtension>();

        Assert.That(core?.IsSensitiveDataLoggingEnabled, Is.False);
    }

    [Test]
    public async Task GetHealth_TokenInQueryString_NoLineHoldsTheToken()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health?token=abc123secret");

        Assert.That(_logs.Lines, Is.Not.Empty);
        Assert.That(_logs.Lines.Select(line => line.AllText), Has.None.Contain("abc123secret"));
    }

    [Test]
    public async Task GetHealth_TwoRequests_EachGetsOneRequestLineWithItsOwnRequestId()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage first = await client.GetAsync("/api/health");
        using HttpResponseMessage second = await client.GetAsync("/api/health");

        LoggedLine[] requestLines = _logs.Lines
            .Where(line => line.Category == RequestLineCategory)
            .ToArray();
        string?[] requestIds = requestLines
            .Select(line => line.Scopes.GetValueOrDefault("RequestId"))
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(requestLines, Has.Length.EqualTo(2));
            Assert.That(requestIds, Has.None.Null.And.Unique);
        }
    }

    [Test]
    public async Task GetHealth_TokenInQueryString_RequestLineHoldsOnlyPathMethodStatusAndDuration()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health?token=abc123secret");

        LoggedLine requestLine = _logs.Lines.Single(line => line.Category == RequestLineCategory);
        Assert.That(
            requestLine.TemplateValues.Keys.Where(key => key != "{OriginalFormat}"),
            Is.EquivalentTo(new[] { "Method", "PathBase", "Path", "StatusCode", "Duration" }));
    }

    [Test]
    public async Task GetHealth_DatabaseUnreachable_LogsErrorLineWithRequestId()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health");

        LoggedLine[] errorLines = _logs.Lines
            .Where(line => line.LogLevel >= LogLevel.Error)
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(errorLines, Is.Not.Empty);
            Assert.That(errorLines, Has.All.Matches<LoggedLine>(line => line.Scopes.ContainsKey("RequestId")));
            Assert.That(errorLines.Select(line => line.AllText), Has.None.Contain("Password=secret"));
        }
    }
}
