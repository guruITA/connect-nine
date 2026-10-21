using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class ProfileDatabaseTests
{
    private string connectionString;
    private ConnectDbContext dbContext;

    [SetUp]
    public void SetUp()
    {
        connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not configured.");

        dbContext = CreateDbContext();
        dbContext.Database.Migrate();
    }

    [TearDown]
    public void TearDown()
    {
        dbContext?.Dispose();
    }

    [Test]
    public void SavingProfile_LeavesOneRow()
    {
        Account account = SaveAccount();
        dbContext.Add(CreateProfile(account));

        dbContext.SaveChanges();

        Assert.That(
            dbContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
            Is.EqualTo(1));
    }

    [Test]
    public void SavingSecondProfileForSameAccount_FailsOnUniqueAccountIndex()
    {
        Account account = SaveAccount();
        dbContext.Add(CreateProfile(account));
        dbContext.SaveChanges();

        using ConnectDbContext secondContext = CreateDbContext();
        Account sameAccount = secondContext.Users.Single(user => user.Id == account.Id);
        secondContext.Add(CreateProfile(sameAccount));

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => secondContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.UniqueViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ix_profile_account_id"));
    }

    [Test]
    public async Task SavingTwoProfilesForSameAccountAtTheSameTime_LeavesOneRow()
    {
        Account account = SaveAccount();
        using var barrier = new Barrier(2);

        Task<bool> firstSave = SaveProfileAtTheSameMoment(account.Id, barrier);
        Task<bool> secondSave = SaveProfileAtTheSameMoment(account.Id, barrier);

        bool[] saves = await Task.WhenAll(firstSave, secondSave);

        using ConnectDbContext verificationContext = CreateDbContext();
        Assert.That(saves.Count(saved => saved), Is.EqualTo(1));
        Assert.That(
            verificationContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
            Is.EqualTo(1));
    }

    [Test]
    public void SavingAgeBand_StoresItsName()
    {
        Account account = SaveAccount();
        Profile profile = CreateProfile(account);
        profile.AgeBand = AgeBand.From18To21;
        dbContext.Add(profile);
        dbContext.SaveChanges();

        string storedAgeBand = dbContext.Database
            .SqlQuery<string>($"SELECT age_band AS \"Value\" FROM profile WHERE id = {profile.Id}")
            .Single();

        Assert.That(storedAgeBand, Is.EqualTo("From18To21"));
    }

    [Test]
    public void InsertingUnknownAgeBand_FailsOnAgeBandCheck()
    {
        Account account = SaveAccount();

        PostgresException exception = Assert.Throws<PostgresException>(
            () => dbContext.Database.ExecuteSql(
                $"""
                INSERT INTO profile (id, account_id, created_at, updated_at, first_name, age_band, region)
                VALUES ({Guid.NewGuid()}, {account.Id}, now(), now(), 'Sam', 'From10To14', 'Amstelveen')
                """)) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");

        Assert.That(exception.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(exception.ConstraintName, Is.EqualTo("ck_profile_age_band"));
    }

    [Test]
    public void SavingEmptyFirstName_FailsOnFirstNameCheck()
    {
        Account account = SaveAccount();
        Profile profile = CreateProfile(account);
        profile.FirstName = string.Empty;
        dbContext.Add(profile);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ck_profile_first_name_not_empty"));
    }

    [Test]
    public void SavingEmptyRegion_FailsOnRegionCheck()
    {
        Account account = SaveAccount();
        Profile profile = CreateProfile(account);
        profile.Region = string.Empty;
        dbContext.Add(profile);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ck_profile_region_not_empty"));
    }

    [Test]
    public void SavingProfileWithoutBio_Succeeds()
    {
        Account account = SaveAccount();
        Profile profile = CreateProfile(account);
        profile.Bio = null;
        dbContext.Add(profile);

        Assert.DoesNotThrow(() => dbContext.SaveChanges());
    }

    private ConnectDbContext CreateDbContext()
    {
        DbContextOptions<ConnectDbContext> options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ConnectDbContext(options);
    }

    private Account SaveAccount()
    {
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

        return account;
    }

    private static Profile CreateProfile(Account account)
    {
        return new Profile
        {
            AccountId = account.Id,
            Account = account,
            FirstName = "Sam",
            AgeBand = AgeBand.From15To17,
            Region = "Amstelveen",
            Bio = "I like football."
        };
    }

    private async Task<bool> SaveProfileAtTheSameMoment(Guid accountId, Barrier barrier)
    {
        await Task.Yield();

        using ConnectDbContext context = CreateDbContext();
        Account account = context.Users.Single(user => user.Id == accountId);
        context.Add(CreateProfile(account));
        barrier.SignalAndWait();

        try
        {
            await context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            return false;
        }
    }

    private static PostgresException FindPostgresException(DbUpdateException exception)
    {
        return exception.InnerException as PostgresException
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
    }
}
