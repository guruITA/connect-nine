using Connect.Application;
using Connect.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Tests;

public sealed class CrossProjectTests
{
    [Test]
    public void ApplicationAndInfrastructureRegistration_Compose()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder().Build();

        Assert.That(services.AddApplication(), Is.SameAs(services));
        Assert.That(services.AddInfrastructure(configuration), Is.SameAs(services));
    }
}
