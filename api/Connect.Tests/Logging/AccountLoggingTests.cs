using System.Net;
using System.Text;
using System.Text.Json;
using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Connect.Tests.Logging;

// The rules of the log of #312 for POST /api/accounts, the first address that receives personal data.
[TestFixture]
public sealed class AccountLoggingTests
{
    private const string Email = "member@example.com";
    private const string Password = "Valid-password-123!";
    private const string ErrorHandlerCategory = "Connect.Presentation.Common.Errors.ExceptionHandlingMiddleware";
    private const string RequestLineCategory = "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware";

    [Test]
    public async Task PostAccount_AccountCreated_NoLineHoldsPasswordOrEmail()
    {
        ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>> handler = CreateHandler();
        handler.HandleAsync(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreateAccountResult>.Success(new CreateAccountResult(Guid.NewGuid(), Email)));
        LoggerSubstitutes logs = new();
        using WebApplicationFactory<Program> factory = CreateFactory(handler, logs);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync("/api/accounts/", JsonContent());

        string[] lines = logs.Lines.Select(line => line.AllText).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(lines, Is.Not.Empty);
            Assert.That(lines, Has.None.Contain(Password));
            Assert.That(lines, Has.None.Contain(Email));
        }
    }

    [Test]
    public async Task PostAccount_HandlerThrows_LogsErrorOnceWithRequestIdAndNoPersonalData()
    {
        ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>> handler = CreateHandler();
        handler.HandleAsync(Arg.Any<CreateAccountCommand>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Something went wrong in the handler."));
        LoggerSubstitutes logs = new();
        using WebApplicationFactory<Program> factory = CreateFactory(handler, logs);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync("/api/accounts/", JsonContent());

        LoggedLine[] errorLines = logs.Lines
            .Where(line => line.LogLevel >= LogLevel.Error)
            .ToArray();
        LoggedLine requestLine = logs.Lines.Single(line => line.Category == RequestLineCategory);
        string? requestId = requestLine.Scopes.GetValueOrDefault("RequestId");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(errorLines, Has.Length.EqualTo(1));
            Assert.That(errorLines.Select(line => line.Category), Has.All.EqualTo(ErrorHandlerCategory));
            Assert.That(errorLines.Select(line => line.Exception), Has.All.InstanceOf<InvalidOperationException>());
            Assert.That(requestId, Is.Not.Null);
            Assert.That(errorLines.Select(line => line.Scopes.GetValueOrDefault("RequestId")), Has.All.EqualTo(requestId));
            Assert.That(requestLine.TemplateValues.GetValueOrDefault("StatusCode"), Is.EqualTo("500"));
            Assert.That(logs.Lines.Select(line => line.AllText), Has.None.Contain(Password));
            Assert.That(logs.Lines.Select(line => line.AllText), Has.None.Contain(Email));
        }
    }

    private static ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>> CreateHandler() =>
        Substitute.For<ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>>();

    // The handler is replaced, so the test needs no database: the log comes from the whole server.
    private static WebApplicationFactory<Program> CreateFactory(
        ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>> handler,
        LoggerSubstitutes logs)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder
            .UseSetting("ConnectionStrings:DefaultConnection", "Host=unused;Database=unused")
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>>();
                services.AddSingleton(handler);
                services.AddSingleton(logs.Provider);
            }));
    }

    private static StringContent JsonContent() =>
        new(
            JsonSerializer.Serialize(new { email = Email, password = Password }),
            Encoding.UTF8,
            "application/json");
}
