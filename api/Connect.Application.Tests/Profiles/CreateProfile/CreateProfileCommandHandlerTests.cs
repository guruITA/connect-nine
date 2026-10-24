using System.Data.Common;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.CreateProfile;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.CreateProfile;

public sealed class CreateProfileCommandHandlerTests
{
    private SqliteConnection connection;
    private TestAppDbContext dbContext;
    private CreateProfileCommandHandler handler;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        dbContext = CreateDbContext(connection);
        dbContext.Database.EnsureCreated();
        handler = new CreateProfileCommandHandler(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_FirstProfile_SavesItAndReturnsItsId()
    {
        Account account = SaveAccount();

        var result = await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Sam", AgeBand.YoungAdult, "Amstelveen", null),
            CancellationToken.None);

        Profile saved = dbContext.Profiles.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value?.Id, Is.EqualTo(saved.Id));
            Assert.That(saved.AccountId, Is.EqualTo(account.Id));
            Assert.That(saved.FirstName, Is.EqualTo("Sam"));
            Assert.That(saved.AgeBand, Is.EqualTo(AgeBand.YoungAdult));
            Assert.That(saved.Region, Is.EqualTo("Amstelveen"));
            Assert.That(saved.Bio, Is.Null);
        }
    }

    [Test]
    public async Task HandleAsync_SecondProfileForSameAccount_ReturnsConflictAndSavesNothing()
    {
        Account account = SaveAccount();
        await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Sam", AgeBand.Adolescent, "Amstelveen", null),
            CancellationToken.None);

        var result = await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Alex", AgeBand.Adult, "Uithoorn", "Hi"),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.Conflict));
            Assert.That(result.Error.Description, Is.EqualTo("You already have a profile"));
            Assert.That(dbContext.Profiles.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task HandleAsync_UnknownAccount_ReturnsUnauthorizedAndSavesNothing()
    {
        var result = await handler.HandleAsync(
            new CreateProfileCommand(Guid.NewGuid(), "Sam", AgeBand.Adolescent, "Amstelveen", null),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.Unauthorized));
            Assert.That(dbContext.Profiles.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void HandleAsync_DatabaseNotReachable_ThrowsTheDatabaseException()
    {
        using var unreachable = new SqliteConnection("Data Source=/nonexistent/connect.db;Mode=ReadOnly");
        using TestAppDbContext unreachableContext = CreateDbContext(unreachable);
        var unreachableHandler = new CreateProfileCommandHandler(unreachableContext);

        Assert.ThrowsAsync(
            Is.InstanceOf<DbException>(),
            () => unreachableHandler.HandleAsync(
                new CreateProfileCommand(Guid.NewGuid(), "Sam", AgeBand.Adolescent, "Amstelveen", null),
                CancellationToken.None));
    }

    private static TestAppDbContext CreateDbContext(SqliteConnection sqliteConnection)
    {
        DbContextOptions<TestAppDbContext> options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseSqlite(sqliteConnection)
            .Options;

        return new TestAppDbContext(options);
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

    private sealed class TestAppDbContext(DbContextOptions<TestAppDbContext> options)
        : IdentityDbContext<Account, IdentityRole<Guid>, Guid>(options), IAppDbContext
    {
        public IQueryable<Account> Accounts => Set<Account>();

        public IQueryable<Profile> Profiles => Set<Profile>();

        void IAppDbContext.Add<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Add(entity);
        }

        void IAppDbContext.Remove<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
        }

        // SQLite raises no PostgreSQL unique violation, so the conflict on a save is tested
        // against PostgreSQL in AppDbContextDatabaseTests and MemberProfileEndpointTests.
        async Task<Result> IAppDbContext.SaveChangesOrConflictAsync(CancellationToken cancellationToken)
        {
            await SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        Task<TEntity?> IAppDbContext.SingleOrDefaultAsync<TEntity>(
            IQueryable<TEntity> query,
            CancellationToken cancellationToken) where TEntity : default
        {
            return query.SingleOrDefaultAsync(cancellationToken);
        }

        Task<bool> IAppDbContext.AnyAsync<TEntity>(
            IQueryable<TEntity> query,
            CancellationToken cancellationToken)
        {
            return query.AnyAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(profile => profile.Id);
                entity.Property(profile => profile.AgeBand).HasConversion<string>();
                entity.HasOne(profile => profile.Account)
                    .WithOne(account => account.Profile)
                    .HasForeignKey<Profile>(profile => profile.AccountId);
            });
        }
    }
}
