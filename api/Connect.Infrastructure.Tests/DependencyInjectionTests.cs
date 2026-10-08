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
        IConfiguration configuration = new ConfigurationBuilder().Build();

        Assert.That(services.AddInfrastructure(configuration), Is.SameAs(services));
    }
}
