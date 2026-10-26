using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

// The members of IAppDbContext that run a query or a save for Application, which has no EF Core
// (decision 007), on PostgreSQL itself.
[TestFixture]
public sealed class AppDbContextDatabaseTests
{
    private ConnectDbContext dbContext;

    [SetUp]
    public void SetUp()
    {
        dbContext = TestDatabase.CreateDbContext();
        dbContext.MigrateAndReloadTypes();
    }

    [TearDown]
    public void TearDown()
    {
        dbContext?.Dispose();
    }

    [Test]
    public async Task SaveChangesOrConflictAsync_NewProfile_ReturnsSuccessAndSavesIt()
    {
        Account account = TestRows.SaveAccount(dbContext);
        IAppDbContext appDbContext = dbContext;
        appDbContext.Add(TestRows.NewProfile(account));

        Result saved = await appDbContext.SaveChangesOrConflictAsync(CancellationToken.None);

        using ConnectDbContext verificationContext = TestDatabase.CreateDbContext();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(saved.IsSuccess, Is.True);
            Assert.That(
                verificationContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
                Is.EqualTo(1));
        }
    }

    [Test]
    public async Task SaveChangesOrConflictAsync_SecondProfileForSameAccount_ReturnsConflict()
    {
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));
        await dbContext.SaveChangesAsync();
        using ConnectDbContext secondContext = TestDatabase.CreateDbContext();
        IAppDbContext appDbContext = secondContext;
        Account sameAccount = secondContext.Users.Single(user => user.Id == account.Id);
        appDbContext.Add(TestRows.NewProfile(sameAccount, "Alex"));

        Result saved = await appDbContext.SaveChangesOrConflictAsync(CancellationToken.None);

        using ConnectDbContext verificationContext = TestDatabase.CreateDbContext();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(saved.IsSuccess, Is.False);
            Assert.That(saved.Error.Code, Is.EqualTo(ErrorCode.Conflict));
            Assert.That(
                verificationContext.Set<Profile>().Count(profile => profile.AccountId == account.Id),
                Is.EqualTo(1));
        }
    }

    [Test]
    public void SaveChangesOrConflictAsync_RefusedByCheckConstraint_ThrowsTheDatabaseError()
    {
        Account account = TestRows.SaveAccount(dbContext);
        IAppDbContext appDbContext = dbContext;
        appDbContext.Add(TestRows.NewProfile(account, firstName: ""));

        DbUpdateException exception = Assert.ThrowsAsync<DbUpdateException>(
            () => appDbContext.SaveChangesOrConflictAsync(CancellationToken.None))
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
        PostgresException postgresException = TestDatabase.FindPostgresException(exception);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(postgresException.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
            Assert.That(postgresException.ConstraintName, Is.EqualTo("ck_profile_first_name_not_empty"));
        }
    }

    [Test]
    public async Task SingleOrDefaultAsync_OneMatchingRow_ReturnsIt()
    {
        Account account = TestRows.SaveAccount(dbContext);
        using ConnectDbContext readContext = TestDatabase.CreateDbContext();
        IAppDbContext appDbContext = readContext;

        Account? found = await appDbContext.SingleOrDefaultAsync(
            appDbContext.Accounts.Where(candidate => candidate.Id == account.Id),
            CancellationToken.None);

        Assert.That(found?.Email, Is.EqualTo(account.Email));
    }

    [Test]
    public async Task SingleOrDefaultAsync_NoMatchingRow_ReturnsNull()
    {
        IAppDbContext appDbContext = dbContext;
        Guid unknownId = Guid.NewGuid();

        Account? found = await appDbContext.SingleOrDefaultAsync(
            appDbContext.Accounts.Where(candidate => candidate.Id == unknownId),
            CancellationToken.None);

        Assert.That(found, Is.Null);
    }

    [Test]
    public async Task AnyAsync_MatchingRow_ReturnsTrue()
    {
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));
        await dbContext.SaveChangesAsync();
        IAppDbContext appDbContext = dbContext;

        bool found = await appDbContext.AnyAsync(
            appDbContext.Profiles.Where(profile => profile.AccountId == account.Id),
            CancellationToken.None);

        Assert.That(found, Is.True);
    }

    [Test]
    public async Task AnyAsync_NoMatchingRow_ReturnsFalse()
    {
        IAppDbContext appDbContext = dbContext;
        Guid unknownId = Guid.NewGuid();

        bool found = await appDbContext.AnyAsync(
            appDbContext.Profiles.Where(profile => profile.AccountId == unknownId),
            CancellationToken.None);

        Assert.That(found, Is.False);
    }

    [Test]
    public void AsNoTracking_ReadProfile_IsNotTracked()
    {
        Account account = TestRows.SaveAccount(dbContext);
        dbContext.Add(TestRows.NewProfile(account));
        dbContext.SaveChanges();
        using ConnectDbContext readContext = TestDatabase.CreateDbContext();
        IAppDbContext appDbContext = readContext;

        Profile read = appDbContext.AsNoTracking(appDbContext.Profiles)
            .Single(profile => profile.AccountId == account.Id);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(read.FirstName, Is.EqualTo("Sam"));
            Assert.That(readContext.ChangeTracker.Entries(), Is.Empty);
        }
    }

    [Test]
    public async Task SingleAsync_OneMatchingRow_ReturnsIt()
    {
        Tag tag = TestRows.SaveTag(dbContext);
        IAppDbContext appDbContext = dbContext;

        Tag found = await appDbContext.SingleAsync(
            appDbContext.Tags.Where(candidate => candidate.Name == tag.Name),
            CancellationToken.None);

        Assert.That(found.Id, Is.EqualTo(tag.Id));
    }

    [Test]
    public void SingleAsync_NoMatchingRow_Throws()
    {
        IAppDbContext appDbContext = dbContext;
        string unknownName = $"n{Guid.NewGuid():N}"[..20];

        Assert.ThrowsAsync<InvalidOperationException>(() => appDbContext.SingleAsync(
            appDbContext.Tags.Where(candidate => candidate.Name == unknownName),
            CancellationToken.None));
    }

    [Test]
    public async Task CountAsync_TheStarterTags_CountsTwenty()
    {
        IAppDbContext appDbContext = dbContext;

        int count = await appDbContext.CountAsync(
            appDbContext.Tags.Where(tag => tag.IsSuggested),
            CancellationToken.None);

        Assert.That(count, Is.EqualTo(20));
    }

    [Test]
    public async Task ToListAsync_TheStarterTags_ReturnsThemInTheOrderAsked()
    {
        IAppDbContext appDbContext = dbContext;

        List<string> names = await appDbContext.ToListAsync(
            appDbContext.Tags.Where(tag => tag.IsSuggested).OrderBy(tag => tag.Name).Select(tag => tag.Name).Take(3),
            CancellationToken.None);

        Assert.That(names, Is.EqualTo(new[] { "coding", "cooking", "dance" }));
    }
}
