using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class TestDataSeederTests
{
    private const string TestMemberEmail = "test-member@example.org";

    private string connectionString = null!;

    [SetUp]
    public void SetUp()
    {
        connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not configured.");

        using ConnectDbContext dbContext = CreateDbContext();
        dbContext.Database.Migrate();
        RemoveTestMember(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        using ConnectDbContext dbContext = CreateDbContext();
        RemoveTestMember(dbContext);
    }

    [Test]
    public async Task SeedAsync_OneRun_AddsOneAccount()
    {
        (TestDataSeeder seeder, _) = CreateSeeder("Correct-Horse-9!");

        await seeder.SeedAsync();

        using ConnectDbContext dbContext = CreateDbContext();
        Assert.That(CountTestMembers(dbContext), Is.EqualTo(1));
    }

    [Test]
    public async Task SeedAsync_TwoRuns_StillOneAccountAndTheSecondRunIsSilent()
    {
        (TestDataSeeder firstSeeder, _) = CreateSeeder("Correct-Horse-9!");
        await firstSeeder.SeedAsync();

        (TestDataSeeder secondSeeder, List<string> secondRunWarnings) =
            CreateSeeder("Correct-Horse-9!");
        await secondSeeder.SeedAsync();

        using ConnectDbContext dbContext = CreateDbContext();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(CountTestMembers(dbContext), Is.EqualTo(1));
            Assert.That(secondRunWarnings, Is.Empty);
        }
    }

    [Test]
    public async Task SeedAsync_OneRun_StoresAHashTheHasherAccepts()
    {
        const string password = "Correct-Horse-9!";
        (TestDataSeeder seeder, _) = CreateSeeder(password);

        await seeder.SeedAsync();

        using ConnectDbContext dbContext = CreateDbContext();
        Account account = await dbContext.Accounts.SingleAsync(
            account => account.Email == TestMemberEmail);
        IPasswordHasher<Account> hasher = new PasswordHasher<Account>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(account.PasswordHash, Is.Not.EqualTo(password));
            Assert.That(
                hasher.VerifyHashedPassword(account, account.PasswordHash!, password),
                Is.EqualTo(PasswordVerificationResult.Success));
        }
    }

    [Test]
    public async Task SeedAsync_EmptyPasswordSetting_AddsNothing()
    {
        (TestDataSeeder seeder, _) = CreateSeeder(string.Empty);

        await seeder.SeedAsync();

        using ConnectDbContext dbContext = CreateDbContext();
        Assert.That(CountTestMembers(dbContext), Is.EqualTo(0));
    }

    [Test]
    public async Task SeedAsync_ExistingAccount_IsNeverChangedEvenWithANewPassword()
    {
        (TestDataSeeder firstSeeder, _) = CreateSeeder("Correct-Horse-9!");
        await firstSeeder.SeedAsync();
        using ConnectDbContext before = CreateDbContext();
        Account original = await before.Accounts.SingleAsync(
            account => account.Email == TestMemberEmail);
        Guid originalId = original.Id;
        string? originalHash = original.PasswordHash;

        (TestDataSeeder secondSeeder, _) = CreateSeeder("Another-Password-7!");
        await secondSeeder.SeedAsync();

        using ConnectDbContext after = CreateDbContext();
        Account[] accounts = await after.Accounts
            .Where(account => account.Email == TestMemberEmail)
            .ToArrayAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(accounts, Has.Length.EqualTo(1));
            Assert.That(accounts[0].Id, Is.EqualTo(originalId));
            Assert.That(accounts[0].PasswordHash, Is.EqualTo(originalHash));
        }
    }

    private (TestDataSeeder Seeder, List<string> Warnings) CreateSeeder(string password)
    {
        List<string> warnings = [];
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Seeding:TestMemberPassword"] = password,
            })
            .Build();

        services.AddSingleton(configuration);
        services.AddInfrastructure(configuration);
        services.AddLogging(logging => logging.AddProvider(new WarningCapturingLoggerProvider(warnings)));

        ServiceProvider provider = services.BuildServiceProvider();
        return (provider.GetRequiredService<TestDataSeeder>(), warnings);
    }

    private ConnectDbContext CreateDbContext()
    {
        DbContextOptions<ConnectDbContext> options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ConnectDbContext(options);
    }

    private static int CountTestMembers(ConnectDbContext dbContext)
    {
        return dbContext.Accounts.Count(account => account.Email == TestMemberEmail);
    }

    private static void RemoveTestMember(ConnectDbContext dbContext)
    {
        Account? existing = dbContext.Accounts
            .SingleOrDefault(account => account.Email == TestMemberEmail);
        if (existing is not null)
        {
            dbContext.Remove(existing);
            dbContext.SaveChanges();
        }
    }

    // Collects every warning SeedAsync logs, so a test can assert that an existing test member
    // is left alone without even one log line, not just without a change in the database.
    private sealed class WarningCapturingLoggerProvider(List<string> warnings) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new WarningCapturingLogger(warnings);

        public void Dispose()
        {
        }

        private sealed class WarningCapturingLogger(List<string> warnings) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Warning;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (logLevel == LogLevel.Warning)
                {
                    warnings.Add(formatter(state, exception));
                }
            }
        }
    }
}
