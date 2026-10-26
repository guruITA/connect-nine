using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Common;
using Connect.Domain.ParentConsents;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Connect.Infrastructure.Persistence;

public sealed class ConnectDbContext(DbContextOptions<ConnectDbContext> options)
    : IdentityDbContext<Account, IdentityRole<Guid>, Guid>(options), IAppDbContext
{
    public IQueryable<Account> Accounts => Set<Account>();

    public IQueryable<Profile> Profiles => Set<Profile>();

    public IQueryable<Tag> Tags => Set<Tag>();

    public IQueryable<ProfileTag> ProfileTags => Set<ProfileTag>();


    public IQueryable<ParentConsent> ParentConsents => Set<ParentConsent>();

    void IAppDbContext.Add<TEntity>(TEntity entity) where TEntity : class
    {
        Set<TEntity>().Add(entity);
    }

    void IAppDbContext.Remove<TEntity>(TEntity entity) where TEntity : class
    {
        Set<TEntity>().Remove(entity);
    }

    async Task<Result> IAppDbContext.SaveChangesOrConflictAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Failure(Error.Conflict("This is already saved"));
        }
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

    Task<TEntity> IAppDbContext.SingleAsync<TEntity>(
        IQueryable<TEntity> query,
        CancellationToken cancellationToken)
    {
        return query.SingleAsync(cancellationToken);
    }

    Task<int> IAppDbContext.CountAsync<TEntity>(
        IQueryable<TEntity> query,
        CancellationToken cancellationToken)
    {
        return query.CountAsync(cancellationToken);
    }

    Task<List<TEntity>> IAppDbContext.ToListAsync<TEntity>(
        IQueryable<TEntity> query,
        CancellationToken cancellationToken)
    {
        return query.ToListAsync(cancellationToken);
    }

    IQueryable<TEntity> IAppDbContext.AsNoTracking<TEntity>(IQueryable<TEntity> query)
    {
        return query.AsNoTracking();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditProperties();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditProperties();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(user => user.CreatedAt).IsRequired();
            entity.Property(user => user.UpdatedAt).IsRequired();
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_asp_net_users_password_hash_not_empty",
                "\"password_hash\" IS NULL OR length(\"password_hash\") > 0"));
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(profile => profile.Id);
            entity.Property(profile => profile.CreatedAt).IsRequired();
            entity.Property(profile => profile.UpdatedAt).IsRequired();
            entity.Property(profile => profile.FirstName).IsRequired();
            entity.Property(profile => profile.AgeBand).HasConversion<string>().IsRequired();
            entity.Property(profile => profile.Region).IsRequired();
            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "ck_profile_first_name_not_empty",
                    "length(\"first_name\") > 0");
                table.HasCheckConstraint(
                    "ck_profile_region_not_empty",
                    "length(\"region\") > 0");
                table.HasCheckConstraint(
                    "ck_profile_age_band",
                    "\"age_band\" IN ('Adolescent', 'YoungAdult', 'Adult')");
            });

            entity.HasOne(profile => profile.Account)
                .WithOne(account => account.Profile)
                .HasForeignKey<Profile>(profile => profile.AccountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.ApplyConfiguration(
            new Configurations.AccountIdentityConfiguration());

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(tag => tag.Id);
            entity.Property(tag => tag.Name).HasColumnType("citext").IsRequired();
            entity.Property(tag => tag.IsSuggested).HasDefaultValue(false);
            entity.Property(tag => tag.CreatedAt).IsRequired();
            entity.Property(tag => tag.UpdatedAt).IsRequired();
            entity.HasIndex(tag => tag.Name).IsUnique().HasDatabaseName("ux_tag_name");
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_tag_name_length",
                "char_length(\"name\") BETWEEN 1 AND 30"));
            entity.HasData(StarterTags.All);
        });

        modelBuilder.Entity<ProfileTag>(entity =>
        {
            entity.HasKey(link => link.Id);
            entity.HasIndex(link => new { link.ProfileId, link.TagId })
                .IsUnique()
                .HasDatabaseName("ux_profile_tag_profile_id_tag_id");
            entity.Property(link => link.CreatedAt).IsRequired();
            entity.Property(link => link.UpdatedAt).IsRequired();
            entity.HasOne<Profile>()
                .WithMany()
                .HasForeignKey(link => link.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(link => link.Tag)
                .WithMany()
                .HasForeignKey(link => link.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ParentConsent>(entity =>
        {
            entity.HasKey(consent => consent.Id);
            entity.Property(consent => consent.ParentEmail).HasColumnType("citext").IsRequired();
            entity.Property(consent => consent.TokenHash).IsRequired();
            entity.Property(consent => consent.ExpiresAt).IsRequired();
            entity.Property(consent => consent.CreatedAt).IsRequired();
            entity.Property(consent => consent.UpdatedAt).IsRequired();
            entity.HasIndex(consent => consent.ProfileId)
                .IsUnique()
                .HasDatabaseName("ux_parent_consent_profile_id");
            entity.HasIndex(consent => consent.TokenHash)
                .IsUnique()
                .HasDatabaseName("ux_parent_consent_token_hash");
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_parent_consent_token_hash_length",
                "char_length(\"token_hash\") = 64"));
            entity.HasOne<Profile>()
                .WithOne(profile => profile.ParentConsent)
                .HasForeignKey<ParentConsent>(consent => consent.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void UpdateAuditProperties()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
            }
        }
    }

    private static bool IsUniqueViolation(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return true;
            }
        }

        return false;
    }
}
