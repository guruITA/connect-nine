using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Connect.Tests.Profiles.Endpoints;

// Runs against a real test database (ConnectionStrings__DefaultConnection). Until log in (#15)
// the account comes from the temporary header X-Temporary-Account-Id.
public sealed class MemberProfileEndpointTests
{
    private const string Address = "/api/member-profiles/";
    private const string AccountHeader = "X-Temporary-Account-Id";

    [Test]
    public async Task Post_ValidProfile_ReturnsCreatedWithIdAndLeavesOneRow()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = "Sam", ageBand = "From18To21", region = "Amstelveen", bio = "Hi" }));
        using JsonDocument body = await ReadJson(response);

        Profile saved = ProfilesOf(factory, accountId).Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(body.RootElement.GetProperty("data").GetProperty("id").GetGuid(), Is.EqualTo(saved.Id));
            Assert.That(body.RootElement.GetProperty("meta").EnumerateObject(), Is.Empty);
            Assert.That(response.Headers.Location?.ToString(), Is.EqualTo($"/api/member-profiles/{saved.Id}"));
            Assert.That(saved.AgeBand, Is.EqualTo(AgeBand.From18To21));
        }
    }

    [Test]
    public async Task Post_FieldsWithSpaces_SavesThemTrimmedAndAnEmptyBioAsNull()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = " Sam ", ageBand = "From15To17", region = "  Amstelveen ", bio = "   " }));

        Profile saved = ProfilesOf(factory, accountId).Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(saved.FirstName, Is.EqualTo("Sam"));
            Assert.That(saved.Region, Is.EqualTo("Amstelveen"));
            Assert.That(saved.Bio, Is.Null);
        }
    }

    [Test]
    public async Task Post_WrongFields_ReturnsOneMessagePerFieldAndSavesNothing()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = " ", ageBand = "from18to21", region = "", bio = new string('a', 281) }));
        using JsonDocument body = await ReadJson(response);
        JsonElement errors = body.RootElement.GetProperty("errors");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(errors.GetProperty("firstName")[0].GetString(), Is.EqualTo("Fill in your first name"));
            Assert.That(errors.GetProperty("ageBand")[0].GetString(), Is.EqualTo("Choose your age band"));
            Assert.That(errors.GetProperty("region")[0].GetString(), Is.EqualTo("Fill in your town or area"));
            Assert.That(
                errors.GetProperty("bio")[0].GetString(),
                Is.EqualTo("Your short line can be at most 280 characters"));
            Assert.That(ProfilesOf(factory, accountId), Is.Empty);
        }
    }

    [Test]
    public async Task Post_SecondProfile_ReturnsConflict()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);
        using HttpResponseMessage first = await client.PostAsync(Address, ValidProfile());

        using HttpResponseMessage second = await client.PostAsync(Address, ValidProfile());
        using JsonDocument body = await ReadJson(second);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("You already have a profile"));
            Assert.That(ProfilesOf(factory, accountId), Has.Count.EqualTo(1));
        }
    }

    [Test]
    public async Task Post_TwoProfilesAtTheSameMoment_GivesOneCreatedAndOneConflict()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        HttpResponseMessage[] responses = await Task.WhenAll(
            client.PostAsync(Address, ValidProfile()),
            client.PostAsync(Address, ValidProfile()));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                responses.Select(response => response.StatusCode),
                Is.EquivalentTo(new[] { HttpStatusCode.Created, HttpStatusCode.Conflict }));
            Assert.That(ProfilesOf(factory, accountId), Has.Count.EqualTo(1));
        }

        foreach (HttpResponseMessage response in responses)
        {
            response.Dispose();
        }
    }

    [Test]
    public async Task Post_WithoutAccount_ReturnsUnauthorizedAndSavesNothing()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(Address, ValidProfile());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(ProfilesOf(factory, accountId), Is.Empty);
        }
    }

    [Test]
    public async Task Post_TemporaryAccountSwitchedOff_ReturnsUnauthorizedEvenWithTheHeader()
    {
        using WebApplicationFactory<Program> factory = CreateFactory(temporaryAccount: false);
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(Address, ValidProfile());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(ProfilesOf(factory, accountId), Is.Empty);
        }
    }

    [Test]
    public async Task Post_AccountIdInTheBody_IsIgnored()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        Guid otherAccountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { accountId = otherAccountId, firstName = "Sam", ageBand = "From15To17", region = "Amstelveen" }));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(ProfilesOf(factory, accountId), Has.Count.EqualTo(1));
            Assert.That(ProfilesOf(factory, otherAccountId), Is.Empty);
        }
    }

    [Test]
    public async Task Post_DatabaseNotReachable_ReturnsServiceUnavailableWithoutErrorText()
    {
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .UseSetting("ConnectionStrings:DefaultConnection", "Host=127.0.0.1;Port=1;Database=none;Timeout=2")
                .UseSetting("TemporaryAccount:Enabled", "true"));
        using HttpClient client = ClientFor(factory, Guid.NewGuid());

        using HttpResponseMessage response = await client.PostAsync(Address, ValidProfile());
        string content = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(content, Does.Contain("Connect is not reachable right now"));
            Assert.That(content, Does.Not.Contain("Exception"));
            Assert.That(content, Does.Not.Contain("127.0.0.1"));
        }
    }

    [Test]
    public async Task Post_ValidProfile_WritesNoProfileTextToTheLog()
    {
        var logs = new CapturingLoggerProvider();
        using WebApplicationFactory<Program> factory = CreateFactory(logs: logs);
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = "Qwertyname", ageBand = "From15To17", region = "Qwertytown", bio = "Qwertybio" }));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(logs.Messages, Is.Not.Empty);
            Assert.That(logs.Messages, Has.None.Contains("Qwerty"));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(
        bool temporaryAccount = true,
        CapturingLoggerProvider? logs = null)
    {
        return new ConnectWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("TemporaryAccount:Enabled", temporaryAccount ? "true" : "false");
            builder.ConfigureLogging(logging =>
            {
                if (logs is not null)
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddProvider(logs);
                }
            });
        });
    }

    private static HttpClient ClientFor(WebApplicationFactory<Program> factory, Guid accountId)
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(AccountHeader, accountId.ToString());
        return client;
    }

    private static Guid SaveAccount(WebApplicationFactory<Program> factory)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        string email = $"profile-{Guid.NewGuid():N}@example.com";
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant()
        };

        dbContext.Add(account);
        dbContext.SaveChanges();

        return account.Id;
    }

    private static List<Profile> ProfilesOf(WebApplicationFactory<Program> factory, Guid accountId)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        return dbContext.Set<Profile>().AsNoTracking().Where(profile => profile.AccountId == accountId).ToList();
    }

    private static StringContent ValidProfile()
    {
        return Json(new { firstName = "Sam", ageBand = "From15To17", region = "Amstelveen", bio = "" });
    }

    private static StringContent Json(object body)
    {
        return new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> messages = new();

        public IReadOnlyCollection<string> Messages => messages.ToArray();

        public ILogger CreateLogger(string categoryName)
        {
            return new CapturingLogger(messages);
        }

        public void Dispose()
        {
        }

        private sealed class CapturingLogger(ConcurrentQueue<string> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                messages.Enqueue(formatter(state, exception) + exception);
            }
        }
    }
}
