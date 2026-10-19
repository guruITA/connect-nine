using Connect.Domain.Users;
using Connect.Infrastructure.Health;
using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is not configured.");

        services.AddDbContext<ConnectDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services
            .AddIdentityCore<Account>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ConnectDbContext>();

        // A database that does not answer within three seconds counts as down (#311).
        services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
