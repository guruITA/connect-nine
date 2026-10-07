namespace Connect.Presentation.Users;

public sealed record CreateUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);
