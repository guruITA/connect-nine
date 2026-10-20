using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Results;
using NUnit.Framework;

namespace Connect.Application.Tests.Accounts.CreateAccount;

public sealed class CreateAccountHandlerTests
{
    [Test]
    public async Task HandleAsync_DelegatesRegistrationToService()
    {
        var expected = Result<CreateAccountResult>.Success(
            new CreateAccountResult(Guid.NewGuid(), "member@example.com"));
        var service = new StubRegistrationService(expected);
        var handler = new CreateAccountCommandHandler(service);
        var command = new CreateAccountCommand("member@example.com", "password");
        using var cancellationTokenSource = new CancellationTokenSource();

        Result<CreateAccountResult> result = await handler.HandleAsync(
            command,
            cancellationTokenSource.Token);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(service.Email, Is.EqualTo(command.Email));
            Assert.That(service.Password, Is.EqualTo(command.Password));
            Assert.That(service.CancellationToken, Is.EqualTo(cancellationTokenSource.Token));
        }
    }

    private sealed class StubRegistrationService(
        Result<CreateAccountResult> result)
        : IAccountRegistrationService
    {
        public string? Email { get; private set; }

        public string? Password { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task<Result<CreateAccountResult>> RegisterAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            Email = email;
            Password = password;
            CancellationToken = cancellationToken;
            return Task.FromResult(result);
        }
    }
}
