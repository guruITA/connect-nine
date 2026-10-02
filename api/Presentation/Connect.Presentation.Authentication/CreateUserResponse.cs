namespace Connect.Presentation.Authentication;

public sealed record CreateUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);
