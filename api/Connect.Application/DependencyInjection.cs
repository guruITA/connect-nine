using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.CreateProfile;
using Connect.Application.Profiles.GetCurrentProfile;
using Connect.Application.Profiles.GetPublicProfile;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>,
            CreateAccountCommandHandler>();
        services.AddScoped<
            ICommandHandler<CreateProfileCommand, Result<CreateProfileResult>>,
            CreateProfileCommandHandler>();
        services.AddScoped<
            IQueryHandler<GetCurrentProfileQuery, Result<CurrentProfileResult>>,
            GetCurrentProfileQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetPublicProfileQuery, Result<GetPublicProfileResponse>>,
            GetPublicProfileQueryHandler>();

        return services;
    }
}
