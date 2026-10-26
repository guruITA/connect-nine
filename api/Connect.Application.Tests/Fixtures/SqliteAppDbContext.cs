using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Tests.Fixtures;

// A handler test's own database: in-memory SQLite behind IAppDbContext, as decision 002 says.
internal sealed class SqliteAppDbContext(DbContextOptions<SqliteAppDbContext> options)
    : IdentityDbContext<Account, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    public IQueryable<Account> Accounts => Set<Account>();

    public IQueryable<Profile> Profiles => Set<Profile>();

    public IQueryable<Tag> Tags => Set<Tag>();

    public IQueryable<ProfileTag> ProfileTags => Set<ProfileTag>();

    public static SqliteAppDbContext Create(SqliteConnection connection)
    {
        return new SqliteAppDbContext(new DbContextOptionsBuilder<SqliteAppDbContext>()
            .UseSqlite(connection)
            .Options);
    }

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

    IQueryable<TEntity> IAppDbContext.AsNoTracking<TEntity>(IQueryable<TEntity> query)
    {
        return query.AsNoTracking();
    }

    public Account SaveAccount()
    {
        string email = $"member-{Guid.NewGuid():N}@example.com";
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant()
        };

        Add(account);
        SaveChanges();

        return account;
    }

    public Profile SaveProfile(Account account, string firstName, AgeBand ageBand, string region, string? bio)
    {
        var profile = new Profile
        {
            AccountId = account.Id,
            Account = account,
            FirstName = firstName,
            AgeBand = ageBand,
            Region = region,
            Bio = bio
        };

        Add(profile);
        SaveChanges();

        return profile;
    }

    public Tag SaveTag(string name, bool isSuggested = false)
    {
        var tag = new Tag { Name = name, IsSuggested = isSuggested };
        Add(tag);
        SaveChanges();

        return tag;
    }

    public void SaveLink(Profile profile, Tag tag)
    {
        Add(new ProfileTag { ProfileId = profile.Id, TagId = tag.Id });
        SaveChanges();
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

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(tag => tag.Id);
            entity.HasIndex(tag => tag.Name).IsUnique();
        });

        modelBuilder.Entity<ProfileTag>(entity =>
        {
            entity.HasKey(link => new { link.ProfileId, link.TagId });
            entity.HasOne<Profile>().WithMany().HasForeignKey(link => link.ProfileId);
            entity.HasOne(link => link.Tag).WithMany().HasForeignKey(link => link.TagId);
        });
    }
}
