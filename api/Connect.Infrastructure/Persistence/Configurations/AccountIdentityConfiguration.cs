using Connect.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public sealed class AccountIdentityConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable(table => table.HasCheckConstraint(
            DatabaseConstants.AccountCheckConstraintPasswordHashNotEmpty,
            "\"password_hash\" IS NULL OR length(\"password_hash\") > 0"));

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.HasIndex(account => account.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName(DatabaseConstants.AccountEmailUniqueIndex);

        builder.HasIndex(account => account.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName(DatabaseConstants.AccountUserNameUniqueIndex);
    }
}
