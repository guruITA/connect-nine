using System.Net;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Tests.TestData;

[TestFixture]
public sealed class TestDataSeedingTests
{
    private const string TestMemberEmail = "test-member@example.org";
    private const string TestMemberPassword = "Correct-Horse-9!";

    // Nothing listens on port 5499, so the real database check must fail (see HealthEndpointTests).
    private const string UnreachableDatabase =
        "Host=localhost;Port=5499;Database=connect;Username=connect;Password=secret";

    [SetUp]
    public void SetUp()
    {
        RemoveTestMember();
    }

    [TearDown]
    public void TearDown()
    {
        RemoveTestMember();
    }

    [Test]
    public async Task Start_SeedingOn_AddsTheTestMemberOnceWithAPasswordThatVerifies()
    {
        using WebApplicationFactory<Program> factory = new ConnectWebApplicationFactory().WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", TestDatabase.ConnectionString())
                .UseSetting("Seeding:TestData", "true")
                .UseSetting("Seeding:TestMemberPassword", TestMemberPassword));
        using HttpClient client = factory.CreateClient();
        await client.GetAsync("/api/health");

        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        UserManager<Account> userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<Account>>();
        Account[] accounts = await dbContext.Accounts
            .Where(account => account.Email == TestMemberEmail)
            .ToArrayAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(accounts, Has.Length.EqualTo(1));
            Assert.That(
                await userManager.CheckPasswordAsync(accounts[0], TestMemberPassword),
                Is.True);
        }
    }

    [Test]
    public async Task Start_SeedingOff_AddsNoAccount()
    {
        using WebApplicationFactory<Program> factory = new ConnectWebApplicationFactory().WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", TestDatabase.ConnectionString())
                .UseSetting("Seeding:TestData", "false")
                .UseSetting("Seeding:TestMemberPassword", TestMemberPassword));
        using HttpClient client = factory.CreateClient();
        await client.GetAsync("/api/health");

        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();

        Assert.That(
            dbContext.Accounts.Count(account => account.Email == TestMemberEmail),
            Is.EqualTo(0));
    }

    [Test]
    public async Task Start_Production_AddsNoAccountEvenWithTheSettingOn()
    {
        using WebApplicationFactory<Program> factory = new ConnectWebApplicationFactory().WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", TestDatabase.ConnectionString())
                .UseEnvironment("Production")
                .UseSetting("Seeding:TestData", "true")
                .UseSetting("Seeding:TestMemberPassword", TestMemberPassword));
        using HttpClient client = factory.CreateClient();
        await client.GetAsync("/api/health");

        using IServiceScope scope = factory.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();

        Assert.That(
            dbContext.Accounts.Count(account => account.Email == TestMemberEmail),
            Is.EqualTo(0));
    }

    [Test]
    public async Task Start_DatabaseUnreachable_ServerStartsAndHealthAnswers()
    {
        using WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(webHost => webHost
                .UseSetting("ConnectionStrings:DefaultConnection", UnreachableDatabase)
                .UseSetting("Seeding:TestData", "true")
                .UseSetting("Seeding:TestMemberPassword", TestMemberPassword));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/api/health");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
    }

    private static void RemoveTestMember()
    {
        using ConnectDbContext dbContext = TestDatabase.CreateDbContext();
        dbContext.Database.Migrate();
        Account? existing = dbContext.Accounts
            .SingleOrDefault(account => account.Email == TestMemberEmail);
        if (existing is not null)
        {
            dbContext.Remove(existing);
            dbContext.SaveChanges();
        }
    }
}
