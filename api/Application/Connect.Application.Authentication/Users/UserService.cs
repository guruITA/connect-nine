namespace Connect.Application.Authentication.Users;

public sealed class UserService(ICreateUserHandler createUserHandler)
{
    public Task<CreateUserResult> CreateUserAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        return createUserHandler.HandleAsync(command, cancellationToken);
    }
}
