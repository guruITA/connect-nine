using Connect.Application.Accounts.CreateAccount;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Application.Tests.Accounts.CreateAccount;

public sealed class CreateAccountHandlerTests
{
    private SqliteConnection connection = null!;
    private TestIdentityDbContext dbContext = null!;
    private ServiceProvider serviceProvider = null!;
    private CreateAccountCommandHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        DbContextOptions<TestIdentityDbContext> options =
            new DbContextOptionsBuilder<TestIdentityDbContext>()
                .UseSqlite(connection)
                .Options;

        dbContext = new TestIdentityDbContext(options);
        dbContext.Database.EnsureCreated();

        ServiceCollection services = new();
        services.AddLogging();
        services.AddDbContext<TestIdentityDbContext>(builder => builder.UseSqlite(connection));
        services
            .AddIdentityCore<Account>(identityOptions =>
            {
                identityOptions.User.RequireUniqueEmail = true;
                identityOptions.Password.RequiredLength = 15;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireUppercase = true;
                identityOptions.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<TestIdentityDbContext>();

        serviceProvider = services.BuildServiceProvider();
        handler = new CreateAccountCommandHandler(
            serviceProvider.GetRequiredService<UserManager<Account>>(),
            serviceProvider
                .GetRequiredService<Microsoft.Extensions.Logging.ILogger<CreateAccountCommandHandler>>());
    }

    [TearDown]
    public void TearDown()
    {
        serviceProvider.Dispose();
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_ValidAccount_ReturnsAccountAndStoresOnlyHash()
    {
        const string password = "Valid-password-123!";

        var result = await handler.HandleAsync(
            new CreateAccountCommand("  member@example.com ", password),
            CancellationToken.None);

        Account account = dbContext.Users.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Email, Is.EqualTo("member@example.com"));
            Assert.That(account.Email, Is.EqualTo("member@example.com"));
            Assert.That(account.PasswordHash, Is.Not.Null.And.Not.Empty);
            Assert.That(account.PasswordHash, Is.Not.EqualTo(password));
        }
    }

    [Test]
    public async Task HandleAsync_EmailAlreadyExists_ReturnsConflict()
    {
        const string password = "Valid-password-123!";

        await handler.HandleAsync(
            new CreateAccountCommand("member@example.com", password),
            CancellationToken.None);

        var result = await handler.HandleAsync(
            new CreateAccountCommand("MEMBER@example.com", password),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(Connect.Application.Common.Results.ErrorCode.Conflict));
            Assert.That(result.Error.Description, Is.EqualTo("This email address already has an account"));
            Assert.That(dbContext.Users.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task HandleAsync_IdentityRejectsPassword_ReturnsFailureWithoutSaving()
    {
        var result = await handler.HandleAsync(
            new CreateAccountCommand("member@example.com", "too-short"),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(Connect.Application.Common.Results.ErrorCode.Failure));
            Assert.That(dbContext.Users, Is.Empty);
        }
    }

    private sealed class TestIdentityDbContext(DbContextOptions<TestIdentityDbContext> options)
        : IdentityDbContext<Account, IdentityRole<Guid>, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().Ignore(account => account.Profile);
            modelBuilder.Entity<Account>()
                .HasIndex(account => account.NormalizedEmail)
                .IsUnique();
        }
    }
}
