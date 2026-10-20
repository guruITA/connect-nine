using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests;

public sealed class DependencyInjectionTests
{
    [Test]
    public void AddInfrastructure_ReturnsServices()
    {
        ServiceCollection services = new();
        var configuration = TestConfiguration.Create();

        Assert.That(services.AddInfrastructure(configuration), Is.SameAs(services));
    }
}
