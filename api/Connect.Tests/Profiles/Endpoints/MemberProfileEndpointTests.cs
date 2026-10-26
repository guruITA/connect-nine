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
using NSubstitute;
using NUnit.Framework;

namespace Connect.Tests.Profiles.Endpoints;

// Runs against a real test database (ConnectionStrings:TestsConnection, handed to the server as its
// DefaultConnection). Until log in (#15) the account comes from the temporary header
// X-Temporary-Account-Id.
public sealed class MemberProfileEndpointTests
{
    private const string Address = "/api/member-profiles/";
    private const string AccountHeader = "X-Temporary-Account-Id";
    private const string CurrentAddress = "/api/member-profiles/current";

    [Test]
    public async Task Post_ValidProfile_ReturnsCreatedWithIdAndLeavesOneRow()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = "Sam", ageBand = "YoungAdult", region = "Amstelveen", bio = "Hi" }));
        using JsonDocument body = await ReadJson(response);

        Profile saved = ProfilesOf(factory, accountId).Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(body.RootElement.GetProperty("data").GetProperty("id").GetGuid(), Is.EqualTo(saved.Id));
            Assert.That(body.RootElement.GetProperty("meta").EnumerateObject(), Is.Empty);
            Assert.That(response.Headers.Location?.ToString(), Is.EqualTo($"/api/member-profiles/{saved.Id}"));
            Assert.That(saved.AgeBand, Is.EqualTo(AgeBand.YoungAdult));
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
            Json(new { firstName = " Sam ", ageBand = "Adolescent", region = "  Amstelveen ", bio = "   " }));

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
            Json(new { firstName = " ", ageBand = "youngadult", region = "", bio = new string('a', 281) }));
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
            Json(new { accountId = otherAccountId, firstName = "Sam", ageBand = "Adolescent", region = "Amstelveen" }));

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
        ILogger logger = NewLogger();
        using WebApplicationFactory<Program> factory = CreateFactory(logger: logger);
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(
            Address,
            Json(new { firstName = "Qwertyname", ageBand = "Adolescent", region = "Qwertytown", bio = "Qwertybio" }));

        string[] messages = LoggedMessages(logger);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages, Has.None.Contains("Qwerty"));
        }
    }

    [Test]
    public async Task GetCurrent_WithProfile_ReturnsExactlyTheFourValues()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);
        using HttpResponseMessage saved = await client.PostAsync(
            Address,
            Json(new { firstName = "Sam", ageBand = "YoungAdult", region = "Amstelveen", bio = "" }));

        using HttpResponseMessage response = await client.GetAsync(CurrentAddress);
        using JsonDocument body = await ReadJson(response);
        JsonElement data = body.RootElement.GetProperty("data");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(
                data.EnumerateObject().Select(property => property.Name),
                Is.EqualTo(new[] { "firstName", "ageBand", "region", "bio" }));
            Assert.That(data.GetProperty("firstName").GetString(), Is.EqualTo("Sam"));
            Assert.That(data.GetProperty("ageBand").GetString(), Is.EqualTo("YoungAdult"));
            Assert.That(data.GetProperty("region").GetString(), Is.EqualTo("Amstelveen"));
            Assert.That(data.GetProperty("bio").ValueKind, Is.EqualTo(JsonValueKind.Null));
            Assert.That(body.RootElement.GetProperty("meta").EnumerateObject(), Is.Empty);
        }
    }

    [Test]
    public async Task GetCurrent_WithoutProfile_ReturnsNotFound()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync(CurrentAddress);
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("You have no profile yet"));
        }
    }

    [Test]
    public async Task GetCurrent_WithoutAccount_ReturnsUnauthorized()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(CurrentAddress);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetCurrent_TwoMembers_EachGetsOnlyTheirOwnProfile()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid first = SaveAccount(factory);
        Guid second = SaveAccount(factory);
        using HttpClient firstClient = ClientFor(factory, first);
        using HttpClient secondClient = ClientFor(factory, second);
        using HttpResponseMessage firstSaved = await firstClient.PostAsync(
            Address,
            Json(new { firstName = "Sam", ageBand = "Adolescent", region = "Amstelveen" }));
        using HttpResponseMessage secondSaved = await secondClient.PostAsync(
            Address,
            Json(new { firstName = "Alex", ageBand = "Adult", region = "Uithoorn" }));

        using HttpResponseMessage firstResponse = await firstClient.GetAsync(CurrentAddress);
        using HttpResponseMessage secondResponse = await secondClient.GetAsync(CurrentAddress);
        using JsonDocument firstBody = await ReadJson(firstResponse);
        using JsonDocument secondBody = await ReadJson(secondResponse);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstBody.RootElement.GetProperty("data").GetProperty("firstName").GetString(), Is.EqualTo("Sam"));
            Assert.That(secondBody.RootElement.GetProperty("data").GetProperty("firstName").GetString(), Is.EqualTo("Alex"));
        }
    }

    [Test]
    public async Task GetCurrent_DatabaseNotReachable_ReturnsServiceUnavailableWithoutErrorText()
    {
        using WebApplicationFactory<Program> factory = UnreachableDatabaseFactory();
        using HttpClient client = ClientFor(factory, Guid.NewGuid());

        using HttpResponseMessage response = await client.GetAsync(CurrentAddress);
        string content = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(content, Does.Contain("Connect is not reachable right now"));
            Assert.That(content, Does.Not.Contain("Exception"));
        }
    }

    [Test]
    public async Task GetById_Adult_ReturnsTheFourPublicValuesAndNothingElse()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid profileId = SaveProfile(factory, "Robin", AgeBand.From18To21, "Amstelveen", "Test member, 18 to 21.");
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");
        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument body = JsonDocument.Parse(content);
        JsonElement data = body.RootElement.GetProperty("data");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(
                data.EnumerateObject().Select(property => property.Name),
                Is.EqualTo(new[] { "firstName", "ageBand", "region", "bio" }));
            Assert.That(data.GetProperty("firstName").GetString(), Is.EqualTo("Robin"));
            Assert.That(data.GetProperty("region").GetString(), Is.EqualTo("Amstelveen"));
            Assert.That(content, Does.Not.Contain("email").And.Not.Contain("accountId").And.Not.Contain(profileId.ToString()));
        }
    }

    [Test]
    public async Task GetById_YoungPersonOf15To17_LeavesTheRegionKeyOut()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid profileId = SaveProfile(factory, "Noor", AgeBand.From15To17, "Uithoorn", "Test member, 15 to 17.");
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");
        using JsonDocument body = await ReadJson(response);
        JsonElement data = body.RootElement.GetProperty("data");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(data.TryGetProperty("region", out _), Is.False);
            Assert.That(data.GetProperty("bio").GetString(), Is.EqualTo("Test member, 15 to 17."));
        }
    }

    [Test]
    public async Task GetById_EmptyBio_LeavesTheBioKeyOut()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid profileId = SaveProfile(factory, "Robin", AgeBand.From22To30, "Amstelveen", null);
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");
        using JsonDocument body = await ReadJson(response);

        Assert.That(body.RootElement.GetProperty("data").TryGetProperty("bio", out _), Is.False);
    }

    [TestCase("00000000-0000-4000-8000-000000000999")]
    [TestCase("abc")]
    public async Task GetById_UnknownOrMalformedId_ReturnsNotFound(string id)
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{id}");
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("This member does not exist"));
        }
    }

    [Test]
    public async Task GetById_OwnProfile_GivesTheSamePublicAnswer()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        using HttpClient client = ClientFor(factory, accountId);
        using HttpResponseMessage saved = await client.PostAsync(
            Address,
            Json(new { firstName = "Noor", ageBand = "From15To17", region = "Uithoorn" }));
        using JsonDocument savedBody = await ReadJson(saved);
        string profileId = savedBody.RootElement.GetProperty("data").GetProperty("id").GetString()!;

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");
        using JsonDocument body = await ReadJson(response);

        Assert.That(body.RootElement.GetProperty("data").TryGetProperty("region", out _), Is.False);
    }

    [Test]
    public async Task GetById_WithoutAccount_ReturnsUnauthorized()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid profileId = SaveProfile(factory, "Robin", AgeBand.From18To21, "Amstelveen", null);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetById_DatabaseNotReachable_ReturnsServiceUnavailableWithoutErrorText()
    {
        using WebApplicationFactory<Program> factory = UnreachableDatabaseFactory();
        using HttpClient client = ClientFor(factory, Guid.NewGuid());

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{Guid.NewGuid()}");
        string content = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(content, Does.Not.Contain("Exception"));
        }
    }

    private static Guid SaveProfile(
        WebApplicationFactory<Program> factory,
        string firstName,
        AgeBand ageBand,
        string region,
        string? bio)
    {
        Guid accountId = SaveAccount(factory);
        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        Account account = dbContext.Users.Single(user => user.Id == accountId);
        var profile = new Profile
        {
            AccountId = accountId,
            Account = account,
            FirstName = firstName,
            AgeBand = ageBand,
            Region = region,
            Bio = bio
        };

        dbContext.Add(profile);
        dbContext.SaveChanges();

        return profile.Id;
    }

    private static WebApplicationFactory<Program> UnreachableDatabaseFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
                .UseSetting("ConnectionStrings:DefaultConnection", "Host=127.0.0.1;Port=1;Database=none;Timeout=2")
                .UseSetting("TemporaryAccount:Enabled", "true"));
    }

    private static WebApplicationFactory<Program> CreateFactory(
        bool temporaryAccount = true,
        ILogger? logger = null)
    {
        return new ConnectWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection", TestDatabase.ConnectionString());
            builder.UseSetting("TemporaryAccount:Enabled", temporaryAccount ? "true" : "false");
            builder.ConfigureLogging(logging =>
            {
                if (logger is not null)
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddProvider(LoggerProviderFor(logger));
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
        return Json(new { firstName = "Sam", ageBand = "Adolescent", region = "Amstelveen", bio = "" });
    }

    private static StringContent Json(object body)
    {
        return new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }

    private static ILogger NewLogger()
    {
        ILogger logger = Substitute.For<ILogger>();
        logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        return logger;
    }

    private static ILoggerProvider LoggerProviderFor(ILogger logger)
    {
        ILoggerProvider provider = Substitute.For<ILoggerProvider>();
        provider.CreateLogger(Arg.Any<string>()).Returns(logger);
        return provider;
    }

    // Every message the logger got, written out the way a log line is, with its exception.
    private static string[] LoggedMessages(ILogger logger)
    {
        return logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Select(call => call.GetArguments())
            .Select(arguments =>
                $"{(arguments[4] as Delegate)?.DynamicInvoke(arguments[2], arguments[3])}{arguments[3]}")
            .ToArray();
    }
}
