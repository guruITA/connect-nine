namespace Connect.Application.Accounts.CreateAccount;

public sealed record CreateAccountCommand(
    string Email,
    string Password
);
