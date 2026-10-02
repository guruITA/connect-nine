using Connect.Application.Authentication.Users;
using Connect.Domain.Authentication;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Connect.Infrastructure.Authentication;

public sealed class CreateUserCommandHandler(
    AuthenticationDbContext dbContext,
    IPasswordHasher passwordHasher) : ICreateUserHandler
{
    public async Task<CreateUserResult> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
        {
            throw new DuplicateUserException(email);
        }

        var user = new User(
            Guid.NewGuid(),
            email,
            string.Empty,
            command.DisplayName.Trim(),
            DateTimeOffset.UtcNow);

        var passwordHash = passwordHasher.Hash(user, command.Password);
        user = new User(user.Id, user.Email, passwordHash, user.DisplayName, user.CreatedAt);

        await dbContext.Users.AddAsync(user, cancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateEmailViolation(exception))
        {
            throw new DuplicateUserException(email);
        }

        return new CreateUserResult(user.Id, user.Email, user.DisplayName, user.CreatedAt);
    }

    private static bool IsDuplicateEmailViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: "ux_user_email"
        };
    }
}
