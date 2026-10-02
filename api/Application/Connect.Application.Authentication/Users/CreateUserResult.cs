namespace Connect.Application.Authentication.Users;

public sealed record CreateUserResult(Guid Id, string Email, string DisplayName, DateTimeOffset CreatedAt);
