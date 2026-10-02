namespace Connect.Presentation.Authentication;

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string DisplayName);
