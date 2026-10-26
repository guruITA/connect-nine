using Connect.Application.Common.Abstractions;
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
