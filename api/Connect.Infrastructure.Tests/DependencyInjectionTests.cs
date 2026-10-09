using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests;

public sealed class DependencyInjectionTests
{
    [Test]
    public void AddInfrastructure_ReturnsServices()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=connect;Username=connect;Password=connect"
            })
            .Build();

        Assert.That(services.AddInfrastructure(configuration), Is.SameAs(services));
    }
}
