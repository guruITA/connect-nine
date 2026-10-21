using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Connect.Infrastructure.Persistence;

public static class TestDataSeedingExtensions
{
    // Called once after builder.Build() and before app.Run() (#277). Never runs in Production,
    // even with the setting on, and a database that cannot be reached must not stop the server
    // from starting.
    public static async Task SeedTestDataAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Seeding:TestData"))
        {
            return;
        }

        using IServiceScope scope = app.Services.CreateScope();
        ILogger<TestDataSeeder> logger = scope.ServiceProvider
            .GetRequiredService<ILogger<TestDataSeeder>>();

        if (app.Environment.IsProduction())
        {
            logger.LogWarning("Test data is never added in Production");
            return;
        }

        TestDataSeeder seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        try
        {
            await seeder.SeedAsync();
        }
        catch (Exception exception)
        {
            // SeedAsync only throws when it cannot even reach the database: a failed CreateAsync
            // is handled inside it and never throws. EF Core wraps the real connection error, so
            // this catches whatever type that wrapping takes rather than one fixed exception type.
            logger.LogWarning(
                exception,
                "Test data could not be added, the database is not reachable");
        }
    }
}
