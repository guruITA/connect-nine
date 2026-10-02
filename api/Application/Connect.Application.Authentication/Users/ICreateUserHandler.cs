namespace Connect.Application.Authentication.Users;

public interface ICreateUserHandler
{
    Task<CreateUserResult> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken);
}
