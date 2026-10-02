namespace Connect.Application.Authentication.Users;

public sealed record CreateUserCommand(string Email, string Password, string DisplayName);
