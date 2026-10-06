using Connect.Application.Users.Contracts;
using Connect.Application.Users.DTO;

namespace Connect.Application.Users.Services;

public sealed class UserService(ICreateUserHandler createUserHandler)
{
    public Task<CreateUserResult> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        return createUserHandler.HandleAsync(command, cancellationToken);
    }
}
