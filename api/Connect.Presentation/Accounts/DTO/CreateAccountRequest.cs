namespace Connect.Presentation.Accounts.DTO;

public sealed record CreateAccountRequest(
    string Email,
    string Password
);
