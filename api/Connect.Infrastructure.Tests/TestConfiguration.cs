using Microsoft.Extensions.Configuration;

namespace Connect.Infrastructure.Tests;

internal static class TestConfiguration
{
    public static IConfiguration Create()
    {
        string connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Database=connect;Username=connect;Password=root";

        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();
    }
}
