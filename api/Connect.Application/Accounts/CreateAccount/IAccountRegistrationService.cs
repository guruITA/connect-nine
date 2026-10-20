using Connect.Application.Common.Results;

namespace Connect.Application.Accounts.CreateAccount;

public interface IAccountRegistrationService
{
    Task<Result<CreateAccountResult>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}
