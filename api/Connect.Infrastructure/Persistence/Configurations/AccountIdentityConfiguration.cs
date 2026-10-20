using Connect.Domain.Users;
using Connect.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public sealed class AccountIdentityConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasIndex(account => account.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName(AccountIdentityDatabaseNames.EmailUniqueIndex);

        builder.HasIndex(account => account.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName(AccountIdentityDatabaseNames.UserNameUniqueIndex);
    }
}
