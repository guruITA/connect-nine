using Connect.Application.Users.Contracts;
using Connect.Infrastructure.Persistence;
using Connect.Infrastructure.Persistence.Users;
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
        var connectionString = configuration.GetConnectionString("Authentication")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:Authentication is not configured.");

        services.AddDbContext<ConnectDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICreateUserHandler, CreateUserCommandHandler>();

        return services;
    }
}
