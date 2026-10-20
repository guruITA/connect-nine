using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Connect.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // One request line per call with only these four fields. The query string, headers and
        // bodies stay out, because that is where passwords, emails and tokens live (#312).
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPath
                | HttpLoggingFields.RequestMethod
                | HttpLoggingFields.ResponseStatusCode
                | HttpLoggingFields.Duration;
            options.CombineLogs = true;
        });

        return services;
    }
}
