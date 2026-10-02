using Connect.Application.Authentication.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Application.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        return services;
    }
}
