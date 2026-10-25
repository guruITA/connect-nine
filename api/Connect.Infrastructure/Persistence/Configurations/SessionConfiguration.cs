using Connect.Domain.Sessions;
using Connect.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Connect.Infrastructure.Persistence.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("session", table =>
        {
            table.HasCheckConstraint(
                DatabaseConstants.SessionCheckConstraintTokenHashSha256,
                "length(\"token_hash\") = 64 AND \"token_hash\" ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(session => session.Id);

        builder.Property(session => session.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(session => session.ExpiresAt)
            .IsRequired();

        builder.Property(session => session.CreatedAt)
            .IsRequired();

        builder.Property(session => session.UpdatedAt)
            .IsRequired();

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(session => session.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => session.TokenHash)
            .IsUnique()
            .HasDatabaseName(DatabaseConstants.SessionTokenUniqueIndex);

        builder.HasIndex(session => session.AccountId)
            .HasDatabaseName(DatabaseConstants.SessionAccountIdIndex);
    }
}
