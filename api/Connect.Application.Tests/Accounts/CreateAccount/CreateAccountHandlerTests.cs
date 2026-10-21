using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Results;
using NSubstitute;
using NUnit.Framework;

namespace Connect.Application.Tests.Accounts.CreateAccount;

public sealed class CreateAccountHandlerTests
{
    [Test]
    public async Task HandleAsync_DelegatesRegistrationToService()
    {
        var expected = Result<CreateAccountResult>.Success(
            new CreateAccountResult(Guid.NewGuid(), "member@example.com"));
        var service = Substitute.For<IAccountRegistrationService>();
        var handler = new CreateAccountCommandHandler(service);
        var command = new CreateAccountCommand("member@example.com", "password");
        using var cancellationTokenSource = new CancellationTokenSource();
        service.RegisterAsync(command.Email, command.Password, cancellationTokenSource.Token)
            .Returns(expected);

        Result<CreateAccountResult> result = await handler.HandleAsync(
            command,
            cancellationTokenSource.Token);

        Assert.That(result, Is.EqualTo(expected));
        await service.Received(1).RegisterAsync(
            command.Email,
            command.Password,
            cancellationTokenSource.Token);
    }
}
