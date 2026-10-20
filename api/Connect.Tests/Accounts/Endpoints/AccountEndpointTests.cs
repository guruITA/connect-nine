using System.Data.Common;
using System.Net;
using System.Text;
using System.Text.Json;
using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace Connect.Tests.Accounts.Endpoints;

public sealed class AccountEndpointTests
{
    [Test]
    public async Task PostAccount_ValidRequest_ReturnsCreatedEnvelopeWithoutPassword()
    {
        Guid accountId = Guid.NewGuid();
        const string password = "Valid-password-123!";

        using WebApplicationFactory<Program> factory = CreateFactory(_ =>
            Result<CreateAccountResult>.Success(
                new CreateAccountResult(accountId, "member@example.com")));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/api/accounts/",
            JsonContent("member@example.com", password));
        using JsonDocument body = await ReadJson(response);

        JsonElement data = body.RootElement.GetProperty("data");
        JsonElement meta = body.RootElement.GetProperty("meta");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(data.GetProperty("id").GetGuid(), Is.EqualTo(accountId));
            Assert.That(data.GetProperty("email").GetString(), Is.EqualTo("member@example.com"));
            Assert.That(meta.EnumerateObject(), Is.Empty);
            Assert.That((await response.Content.ReadAsStringAsync()), Does.Not.Contain(password));
        }
    }

    [Test]
    public async Task PostAccount_InvalidRequest_ReturnsValidationProblemWithCamelCaseKeys()
    {
        bool handlerCalled = false;
        using WebApplicationFactory<Program> factory = CreateFactory(_ =>
        {
            handlerCalled = true;
            return Result<CreateAccountResult>.Success(
                new CreateAccountResult(Guid.NewGuid(), "unused@example.com"));
        });
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/api/accounts/",
            JsonContent("", ""));
        using JsonDocument body = await ReadJson(response);
        JsonElement errors = body.RootElement.GetProperty("errors");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(errors.GetProperty("email")[0].GetString(),
                Is.EqualTo("Fill in a valid email address"));
            Assert.That(errors.GetProperty("password")[0].GetString(),
                Is.EqualTo("Fill in a password"));
            Assert.That(errors.EnumerateObject().Select(property => property.Name),
                Is.EquivalentTo(new[] { "email", "password" }));
            Assert.That(handlerCalled, Is.False);
        }
    }

    [Test]
    public async Task PostAccount_ConflictResult_ReturnsConflictProblemDetails()
    {
        using WebApplicationFactory<Program> factory = CreateFactory(_ =>
            Result<CreateAccountResult>.Failure(
                Error.Conflict("This email address already has an account")));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/api/accounts/",
            JsonContent("member@example.com", "Valid-password-123!"));
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/problem+json"));
            Assert.That(body.RootElement.GetProperty("title").GetString(),
                Is.EqualTo("This email address already has an account"));
            Assert.That(body.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(409));
            Assert.That((await response.Content.ReadAsStringAsync()), Does.Not.Contain("Valid-password-123!"));
        }
    }

    [Test]
    public async Task PostAccount_DatabaseException_ReturnsServiceUnavailableProblemDetails()
    {
        using WebApplicationFactory<Program> factory = CreateFactory(_ => throw new TestDatabaseException());
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsync(
            "/api/accounts/",
            JsonContent("member@example.com", "Valid-password-123!"));
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/problem+json"));
            Assert.That(body.RootElement.GetProperty("title").GetString(),
                Is.EqualTo("Connect is not reachable right now"));
            Assert.That((await response.Content.ReadAsStringAsync()), Does.Not.Contain("Valid-password-123!"));
        }
    }

    private static WebApplicationFactory<Program> CreateFactory(
        Func<CreateAccountCommand, Result<CreateAccountResult>> resultFactory)
    {
        var handler = new StubCreateAccountHandler(resultFactory);

        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder
                .UseSetting("ConnectionStrings:DefaultConnection", "Host=unused;Database=unused")
                .ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "Host=unused;Database=unused"
                    }))
                .ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>>();
                    services.AddSingleton<ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>>(handler);
                });
        });
    }

    private static StringContent JsonContent(string email, string password)
    {
        return new StringContent(
            JsonSerializer.Serialize(new { email, password }),
            Encoding.UTF8,
            "application/json");
    }

    private static async Task<JsonDocument> ReadJson(HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content, new JsonDocumentOptions());
    }

    private sealed class StubCreateAccountHandler(
        Func<CreateAccountCommand, Result<CreateAccountResult>> resultFactory)
        : ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>
    {
        public Task<Result<CreateAccountResult>> HandleAsync(
            CreateAccountCommand command,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(resultFactory(command));
        }
    }

    private sealed class TestDatabaseException : DbException
    {
    }
}
