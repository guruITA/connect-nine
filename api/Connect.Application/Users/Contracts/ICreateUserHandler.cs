using Connect.Application.Users.DTO;

namespace Connect.Application.Users.Contracts;

public interface ICreateUserHandler
{
    Task<CreateUserResult> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken);
}
