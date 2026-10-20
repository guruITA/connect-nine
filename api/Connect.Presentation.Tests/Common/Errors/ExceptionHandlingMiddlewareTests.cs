using System.Data.Common;
using System.Net;
using Connect.Presentation.Common.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Common.Errors;

public sealed class ExceptionHandlingMiddlewareTests : IDisposable
{
    [Test]
    public async Task GenericException_ReturnsInternalServerErrorProblemDetails()
    {
        using IHost host = CreateHost(new InvalidOperationException("secret"));
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.GetAsync("/");
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/problem+json"));
            Assert.That(body, Does.Contain("An unexpected error occurred."));
            Assert.That(body, Does.Not.Contain("secret"));
        }
    }

    [Test]
    public async Task DatabaseException_ReturnsServiceUnavailableProblemDetails()
    {
        using IHost host = CreateHost(new TestDatabaseException());
        using HttpClient client = host.GetTestClient();

        using HttpResponseMessage response = await client.GetAsync("/");
        string body = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/problem+json"));
            Assert.That(body, Does.Contain("Connect is not reachable right now"));
        }
    }

    public void Dispose()
    {
    }

    private static IHost CreateHost(Exception exception)
    {
        return new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .ConfigureServices(services => services.AddLogging())
                .Configure(app => app
                    .UseMiddleware<ExceptionHandlingMiddleware>()
                    .Run(_ => throw exception)))
            .Start();
    }

    private sealed class TestDatabaseException : DbException
    {
    }
}
