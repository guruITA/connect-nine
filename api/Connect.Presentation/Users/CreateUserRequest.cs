namespace Connect.Presentation.Users;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string DisplayName);
