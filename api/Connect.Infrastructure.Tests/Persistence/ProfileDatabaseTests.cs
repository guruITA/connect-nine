using Connect.Application.Common.Abstractions;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

[TestFixture]
public sealed class ProfileDatabaseTests
{
    private ConnectDbContext dbContext;

    [SetUp]
    public void SetUp()
    {
        dbContext = TestDatabase.CreateDbContext();
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
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));

        dbContext.SaveChanges();

        Assert.That(
            dbContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
            Is.EqualTo(1));
    }

    [Test]
    public void SavingSecondProfileForSameAccount_FailsOnUniqueAccountIndex()
    {
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));
        dbContext.SaveChanges();

        using ConnectDbContext secondContext = TestDatabase.CreateDbContext();
        Account sameAccount = secondContext.Users.Single(user => user.Id == account.Id);
        secondContext.Add(TestRows.NewProfile(sameAccount));

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => secondContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = TestDatabase.FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.UniqueViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ix_profile_account_id"));
    }

    [Test]
    public async Task SavingTwoProfilesForSameAccountAtTheSameTime_LeavesOneRow()
    {
        Account account = TestRows.SaveAccount(dbContext);
        using var barrier = new Barrier(2);

        Task<bool> firstSave = SaveProfileAtTheSameMoment(account.Id, barrier);
        Task<bool> secondSave = SaveProfileAtTheSameMoment(account.Id, barrier);

        bool[] saves = await Task.WhenAll(firstSave, secondSave);

        using ConnectDbContext verificationContext = TestDatabase.CreateDbContext();
        Assert.That(saves.Count(saved => saved), Is.EqualTo(1));
        Assert.That(
            verificationContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
            Is.EqualTo(1));
    }

    [Test]
    public void SavingAgeBand_StoresItsName()
    {
        Account account = TestRows.SaveAccount(dbContext);
        Profile profile = TestRows.NewProfile(account);
        profile.AgeBand = AgeBand.YoungAdult;
        dbContext.Add(profile);
        dbContext.SaveChanges();

        string storedAgeBand = dbContext.Database
            .SqlQuery<string>($"SELECT age_band AS \"Value\" FROM profile WHERE id = {profile.Id}")
            .Single();

        Assert.That(storedAgeBand, Is.EqualTo("YoungAdult"));
    }

    [Test]
    public void InsertingUnknownAgeBand_FailsOnAgeBandCheck()
    {
        Account account = TestRows.SaveAccount(dbContext);

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
        Account account = TestRows.SaveAccount(dbContext);
        Profile profile = TestRows.NewProfile(account);
        profile.FirstName = string.Empty;
        dbContext.Add(profile);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = TestDatabase.FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ck_profile_first_name_not_empty"));
    }

    [Test]
    public void SavingEmptyRegion_FailsOnRegionCheck()
    {
        Account account = TestRows.SaveAccount(dbContext);
        Profile profile = TestRows.NewProfile(account);
        profile.Region = string.Empty;
        dbContext.Add(profile);

        DbUpdateException exception = Assert.Throws<DbUpdateException>(
            () => dbContext.SaveChanges()) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = TestDatabase.FindPostgresException(exception);

        Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
        Assert.That(postgresException.ConstraintName, Is.EqualTo("ck_profile_region_not_empty"));
    }

    [Test]
    public void SavingProfileWithoutBio_Succeeds()
    {
        Account account = TestRows.SaveAccount(dbContext);
        Profile profile = TestRows.NewProfile(account);
        profile.Bio = null;
        dbContext.Add(profile);

        Assert.DoesNotThrow(() => dbContext.SaveChanges());
    }

    [Test]
    public void FindingTheProfileOfAnAccount_ReturnsItsFourValues()
    {
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));
        dbContext.SaveChanges();

        using ConnectDbContext readContext = TestDatabase.CreateDbContext();
        var found = ProfileOfAccount(readContext, account.Id);

        Assert.That(found, Is.EqualTo(new ProfileValues("Sam", AgeBand.Adolescent, "Amstelveen", "I like football.")));
    }

    [Test]
    public void FindingTheProfileOfAnAccountWithoutOne_ReturnsNothing()
    {
        Account account = TestRows.SaveAccount(dbContext);

        using ConnectDbContext readContext = TestDatabase.CreateDbContext();

        Assert.That(ProfileOfAccount(readContext, account.Id), Is.Null);
    }

    [Test]
    public void FindingTheProfileOfAnAccount_NeverReturnsTheProfileOfAnother()
    {
        Account own = TestRows.SaveAccount(dbContext);
        Account other = TestRows.SaveAccount(dbContext);
        Profile otherProfile = TestRows.NewProfile(other);
        otherProfile.FirstName = "Alex";
        dbContext.Add(TestRows.NewProfile(own));
        dbContext.Add(otherProfile);
        dbContext.SaveChanges();

        using ConnectDbContext readContext = TestDatabase.CreateDbContext();

        Assert.That(ProfileOfAccount(readContext, own.Id)?.FirstName, Is.EqualTo("Sam"));
    }

    // The query of GET /api/member-profiles/current: through IAppDbContext, without tracking and
    // without loading the account.
    private static ProfileValues? ProfileOfAccount(IAppDbContext appDbContext, Guid accountId)
    {
        return appDbContext.AsNoTracking(appDbContext.Profiles)
            .Where(profile => profile.AccountId == accountId)
            .Select(profile => new ProfileValues(profile.FirstName, profile.AgeBand, profile.Region, profile.Bio))
            .SingleOrDefault();
    }

    private sealed record ProfileValues(string FirstName, AgeBand AgeBand, string Region, string? Bio);

    [Test]
    public void ReadingThePublicPartOfAKnownProfile_ReturnsItsFourValuesAndReadsOnlyThoseColumns()
    {
        Account account = TestRows.SaveAccount(dbContext);
        Profile profile = TestRows.NewProfile(account);
        dbContext.Add(profile);
        dbContext.SaveChanges();
        var recorder = new DatabaseCommandRecorder();

        using ConnectDbContext readContext = TestDatabase.CreateDbContext(recorder);
        PublicMemberProfile? found = PublicPartOf(readContext, profile.Id);

        string select = recorder.CommandsFor("profile").Single();
        TestContext.Progress.WriteLine(select);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(found, Is.EqualTo(new PublicMemberProfile("Sam", AgeBand.Adolescent, "Amstelveen", "I like football.")));
            Assert.That(select, Does.Contain("first_name").And.Contain("age_band").And.Contain("region").And.Contain("bio"));
            Assert.That(select, Does.Not.Contain("account_id").And.Not.Contain("created_at").And.Not.Contain("updated_at"));
        }
    }

    [Test]
    public void ReadingThePublicPartOfAnUnknownProfile_ReturnsNothing()
    {
        using ConnectDbContext readContext = TestDatabase.CreateDbContext();

        Assert.That(PublicPartOf(readContext, Guid.NewGuid()), Is.Null);
    }

    // The query of GET /api/member-profiles/{id}: only the four public columns, by the primary key,
    // as a projection that EF Core does not track.
    private static PublicMemberProfile? PublicPartOf(IAppDbContext appDbContext, Guid profileId)
    {
        return appDbContext.Profiles
            .Where(profile => profile.Id == profileId)
            .Select(profile => new PublicMemberProfile(profile.FirstName, profile.AgeBand, profile.Region, profile.Bio))
            .SingleOrDefault();
    }

    private async Task<bool> SaveProfileAtTheSameMoment(Guid accountId, Barrier barrier)
    {
        await Task.Yield();

        using ConnectDbContext context = TestDatabase.CreateDbContext();
        Account account = context.Users.Single(user => user.Id == accountId);
        context.Add(TestRows.NewProfile(account));
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
}
