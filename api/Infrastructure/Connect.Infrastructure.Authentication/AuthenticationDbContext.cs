using Connect.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Connect.Infrastructure.Authentication;

public sealed class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.DisplayName).IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
            entity.HasIndex(user => user.Email)
                .IsUnique()
                .HasDatabaseName("ux_user_email");
        });
    }
}
