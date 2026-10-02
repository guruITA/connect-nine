using Connect.Application.Authentication.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Authentication")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:Authentication is not configured.");

        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICreateUserHandler, CreateUserCommandHandler>();

        return services;
    }
}
