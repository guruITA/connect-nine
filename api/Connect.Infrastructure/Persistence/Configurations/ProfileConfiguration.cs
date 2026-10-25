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

        builder.Property(profile => profile.FirstName)
            .IsRequired();

        builder.Property(profile => profile.AgeBand)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(profile => profile.Region)
            .IsRequired();

        builder.ToTable(table =>
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

        builder.HasOne(profile => profile.Account)
            .WithOne(account => account.Profile)
            .HasForeignKey<Profile>(profile => profile.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
