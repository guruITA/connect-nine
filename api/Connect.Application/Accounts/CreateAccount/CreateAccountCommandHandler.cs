using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Connect.Application.Accounts.CreateAccount;

public sealed class CreateAccountCommandHandler(
    UserManager<Account> userManager,
    ILogger<CreateAccountCommandHandler> logger)
    : ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>>
{
    public async Task<Result<CreateAccountResult>> HandleAsync(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Email.Trim();
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email
        };

        IdentityResult result;
        try
        {
            result = await userManager.CreateAsync(account, command.Password);
        }
        catch (DbUpdateException exception) when (IsEmailUniqueViolation(exception))
        {
            return Result<CreateAccountResult>.Failure(
                Error.Conflict("This email address already has an account"));
        }

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error =>
                    error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return Result<CreateAccountResult>.Failure(
                    Error.Conflict("This email address already has an account"));
            }

            logger.LogError(
                "Account creation failed unexpectedly. Identity errors: {IdentityErrors}",
                result.Errors);

            return Result<CreateAccountResult>.Failure(Error.Failure);
        }

        return Result<CreateAccountResult>.Success(new CreateAccountResult(account.Id, email));
    }

    private static bool IsEmailUniqueViolation(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "ux_account_email"
                })
            {
                return true;
            }
        }

        return false;
    }
}
