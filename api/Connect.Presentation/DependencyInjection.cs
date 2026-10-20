using Connect.Presentation.Accounts.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        ValidatorOptions.Global.PropertyNameResolver = (_, member, _) =>
            member?.Name is null
                ? null
                : char.ToLowerInvariant(member.Name[0]) + member.Name[1..];
        services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();

        return services;
    }
}
