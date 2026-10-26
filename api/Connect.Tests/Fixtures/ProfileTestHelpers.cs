using System.Text;
using System.Text.Json;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Connect.Tests.Fixtures;

// Accounts, profiles and clients for the endpoint tests of the profile addresses (backend rule 20),
// on the test database of ConnectionStrings:TestsConnection, handed to the server as its
// DefaultConnection. Until log in (#15) the account comes from the temporary header
// X-Temporary-Account-Id.
internal static class ProfileTestHelpers
{
    public static WebApplicationFactory<Program> CreateFactory(
        bool temporaryAccount = true,
        ILoggerProvider? loggerProvider = null)
    {
        return new ConnectWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection", TestDatabase.ConnectionString());
            builder.UseSetting("TemporaryAccount:Enabled", temporaryAccount ? "true" : "false");
            builder.ConfigureLogging(logging =>
            {
                if (loggerProvider is not null)
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddProvider(loggerProvider);
                }
            });
        });
    }

    public static WebApplicationFactory<Program> UnreachableDatabaseFactory()
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder
            .UseSetting("ConnectionStrings:DefaultConnection", "Host=127.0.0.1;Port=1;Database=none;Timeout=2")
            .UseSetting("TemporaryAccount:Enabled", "true"));
    }

    public static HttpClient ClientFor(WebApplicationFactory<Program> factory, Guid accountId)
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Temporary-Account-Id", accountId.ToString());
        return client;
    }

    public static Guid SaveAccount(WebApplicationFactory<Program> factory)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        string email = $"member-{Guid.NewGuid():N}@example.com";
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

    public static Guid SaveProfile(
        WebApplicationFactory<Program> factory,
        Guid accountId,
        AgeBand ageBand,
        string firstName = "Sam",
        string region = "Amstelveen",
        string? bio = null)
    {
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

    public static StringContent Json(object body)
    {
        return new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    public static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }
}
