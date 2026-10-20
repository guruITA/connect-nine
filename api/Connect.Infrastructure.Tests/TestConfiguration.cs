using Microsoft.Extensions.Configuration;

namespace Connect.Infrastructure.Tests;

internal static class TestConfiguration
{
    public static IConfiguration Create()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets<ApplicationAssemblyMarker>()
            .AddEnvironmentVariables()
            .Build();
    }
}
