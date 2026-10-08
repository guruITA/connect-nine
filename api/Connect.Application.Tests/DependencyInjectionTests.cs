using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Application.Tests;

public sealed class DependencyInjectionTests
{
    [Test]
    public void AddApplication_ReturnsServices()
    {
        ServiceCollection services = new();

        Assert.That(services.AddApplication(), Is.SameAs(services));
    }
}
