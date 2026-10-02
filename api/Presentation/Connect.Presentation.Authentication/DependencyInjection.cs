using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Presentation.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        return services;
    }
}
