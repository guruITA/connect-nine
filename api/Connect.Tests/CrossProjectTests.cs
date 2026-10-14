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
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=connect;Username=connect;Password=connect"
            })
            .Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(services.AddApplication(), Is.Not.SameAs(services));
            Assert.That(services.AddInfrastructure(configuration), Is.SameAs(services));
        }
    }
}
