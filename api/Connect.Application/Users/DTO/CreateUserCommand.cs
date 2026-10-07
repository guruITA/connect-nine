namespace Connect.Application.Users.DTO;

public sealed record CreateUserCommand(string Email, string Password, string DisplayName);
