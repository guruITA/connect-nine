using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Results;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Connect.Infrastructure.Identity;

public sealed class AccountRegistrationService(UserManager<Account> userManager, ILogger<AccountRegistrationService> logger) : IAccountRegistrationService
{
    private readonly UserManager<Account> _userManager = userManager;
    private readonly ILogger<AccountRegistrationService> _logger = logger;

    public async Task<Result<CreateAccountResult>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var trimmedEmail = email.Trim();

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = trimmedEmail,
            UserName = trimmedEmail
        };

        IdentityResult result;

        try
        {
            result = await _userManager.CreateAsync(account, password);
        }
        catch (DbUpdateException exception) when (IsAccountUniqueViolation(exception))
        {
            return DuplicateAccount();
        }

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error =>
                    error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return DuplicateAccount();
            }

            _logger.LogWarning(
                "Account registration was rejected by Identity. Error codes: {IdentityErrorCodes}",
                string.Join(", ", result.Errors.Select(error => error.Code)));

            return Result<CreateAccountResult>.Failure(Error.Failure);
        }

        return Result<CreateAccountResult>.Success(
            new CreateAccountResult(account.Id, trimmedEmail));
    }

    private static Result<CreateAccountResult> DuplicateAccount()
    {
        return Result<CreateAccountResult>.Failure(
            Error.Conflict("This email address already has an account"));
    }

    private static bool IsAccountUniqueViolation(Exception exception)
    {
        for (Exception? current = exception;
             current is not null;
             current = current.InnerException)
        {
            var isAccountUniqueViolation = current is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName:
                    AccountIdentityDatabaseNames.EmailUniqueIndex or
                    AccountIdentityDatabaseNames.UserNameUniqueIndex
            };

            if (isAccountUniqueViolation)
            {
                return true;
            }
        }

        return false;
    }
}
