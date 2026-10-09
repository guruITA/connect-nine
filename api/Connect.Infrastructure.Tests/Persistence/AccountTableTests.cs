using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class AccountTableTests
{
    private ConnectDbContext dbContext = null!;

    [SetUp]
    public void SetUp()
    {
        string connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not configured.");

        DbContextOptions<ConnectDbContext> options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        dbContext = new ConnectDbContext(options);
        dbContext.Database.Migrate();
    }

    [TearDown]
    public void TearDown()
    {
        dbContext?.Dispose();
    }

    [Test]
    public void SavingSameEmailWithDifferentCapitalization_FailsOnUniqueEmailConstraint()
    {
        string email = $"account-{Guid.NewGuid():N}@example.com";
        Account first = CreateAccount(email, email.ToUpperInvariant());
        Account second = CreateAccount(
            email.ToUpperInvariant(),
            email.ToUpperInvariant());
        second.UserName = $"other-{Guid.NewGuid():N}";
        second.NormalizedUserName = second.UserName.ToUpperInvariant();

        dbContext.Add(first);
        dbContext.SaveChanges();
        dbContext.Add(second);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges())!;
        PostgresException postgresException = FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.UniqueViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ux_account_email"));
    }

    [Test]
    public void SavingEmptyPasswordHash_FailsOnPasswordHashCheck()
    {
        Account account = CreateAccount(
            $"account-{Guid.NewGuid():N}@example.com",
            Guid.NewGuid().ToString("N"));
        account.PasswordHash = string.Empty;
        dbContext.Add(account);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges())!;
        PostgresException postgresException = FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(
            postgresException.ConstraintName,
            Is.EqualTo("ck_asp_net_users_password_hash_not_empty"));
    }

    [Test]
    public void SavingAccountWithPasswordHash_Succeeds()
    {
        Account account = CreateAccount(
            $"account-{Guid.NewGuid():N}@example.com",
            Guid.NewGuid().ToString("N"));
        account.PasswordHash = "hash";

        dbContext.Add(account);

        Assert.DoesNotThrow(() => dbContext.SaveChanges());
    }

    [Test]
    public void SavingAccountWithNullPasswordHash_Succeeds()
    {
        Account account = CreateAccount(
            $"account-{Guid.NewGuid():N}@example.com",
            Guid.NewGuid().ToString("N"));

        dbContext.Add(account);

        Assert.DoesNotThrow(() => dbContext.SaveChanges());
    }

    private static Account CreateAccount(string email, string normalizedEmail)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            UserName = email,
            NormalizedUserName = normalizedEmail
        };
    }

    private static PostgresException FindPostgresException(DbUpdateException exception)
    {
        return exception.InnerException as PostgresException
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
    }
}
