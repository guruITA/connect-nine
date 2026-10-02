namespace Connect.Application.Users.DTO;

public sealed record CreateUserResult(Guid Id, string Email, string DisplayName, DateTimeOffset CreatedAt);
