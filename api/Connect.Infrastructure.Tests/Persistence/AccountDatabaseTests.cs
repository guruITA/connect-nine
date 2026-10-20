using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

[TestFixture]
public sealed class AccountDatabaseTests
{
    private ConnectDbContext dbContext;
    private string connectionString;

    [SetUp]
    public void SetUp()
    {
        IConfiguration configuration = TestConfiguration.Create();
        connectionString = configuration.GetConnectionString("TestsConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:TestsConnection is not configured.");

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
    public void FindingEmailWithDifferentCapitalization_UsesNormalizedEmail()
    {
        string email = $"account-{Guid.NewGuid():N}@example.com";
        Account account = CreateAccount(email, email.ToUpperInvariant());
        account.PasswordHash = "hash";

        dbContext.Add(account);
        dbContext.SaveChanges();

        Account? found = dbContext.Users.SingleOrDefault(
            candidate => candidate.NormalizedEmail == email.ToUpperInvariant());

        Assert.That(found?.Id, Is.EqualTo(account.Id));
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
            () => dbContext.SaveChanges());
        PostgresException postgresException = FindPostgresException(exception);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
            Assert.That(
                postgresException.ConstraintName,
                Is.EqualTo("ck_asp_net_users_password_hash_not_empty"));
        }
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

    [Test]
    public async Task SavingSameEmailAtTheSameTime_LeavesOneRow()
    {
        string email = $"account-{Guid.NewGuid():N}@example.com";
        using var barrier = new Barrier(2);

        Task<bool> firstSave = SaveAtTheSameMoment(email, barrier);
        Task<bool> secondSave = SaveAtTheSameMoment(email, barrier);

        bool[] saves = await Task.WhenAll(firstSave, secondSave);

        using ConnectDbContext verificationContext = CreateDbContext();
        int rowCount = verificationContext.Users.Count(
            account => account.NormalizedEmail == email.ToUpperInvariant());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(saves.Count(saveSucceeded => saveSucceeded), Is.EqualTo(1));
            Assert.That(rowCount, Is.EqualTo(1));
        }
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

    private Task<bool> SaveAtTheSameMoment(string email, Barrier barrier)
    {
        return Task.Run(() =>
        {
            using ConnectDbContext context = CreateDbContext();
            string normalizedEmail = email.ToUpperInvariant();
            Account account = CreateAccount(email, normalizedEmail);
            account.UserName = $"user-{Guid.NewGuid():N}";
            account.NormalizedUserName = account.UserName.ToUpperInvariant();
            account.PasswordHash = "hash";

            context.Add(account);
            barrier.SignalAndWait();

            try
            {
                context.SaveChanges();
                return true;
            }
            catch (DbUpdateException exception) when (IsEmailUniqueViolation(exception))
            {
                return false;
            }
        });
    }

    private ConnectDbContext CreateDbContext()
    {
        DbContextOptions<ConnectDbContext> options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ConnectDbContext(options);
    }

    private static bool IsEmailUniqueViolation(DbUpdateException exception)
    {
        PostgresException postgresException = FindPostgresException(exception);
        return postgresException.SqlState == PostgresErrorCodes.UniqueViolation
            && postgresException.ConstraintName == "ux_account_email";
    }

    private static PostgresException FindPostgresException(DbUpdateException exception)
    {
        return exception.InnerException as PostgresException
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
    }
}
