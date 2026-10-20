using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;

namespace Connect.Application.Accounts.CreateAccount;

public sealed class CreateAccountCommandHandler(IAccountRegistrationService registrationService) : ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>
{
    private readonly IAccountRegistrationService _registrationService = registrationService;

    public Task<Result<CreateAccountResult>> HandleAsync(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        return _registrationService.RegisterAsync(
            command.Email,
            command.Password,
            cancellationToken);
    }
}
