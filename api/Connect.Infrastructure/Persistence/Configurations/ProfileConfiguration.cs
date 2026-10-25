using Connect.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Connect.Infrastructure.Persistence.Configurations;

public sealed class ProfileConfiguration
    : IEntityTypeConfiguration<Profile>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.CreatedAt)
            .IsRequired();

        builder.Property(profile => profile.UpdatedAt)
            .IsRequired();

        builder.HasOne(profile => profile.Account)
            .WithOne(account => account.Profile)
            .HasForeignKey<Profile>(profile => profile.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
