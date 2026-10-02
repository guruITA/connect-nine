namespace Connect.Application.Authentication.Users;

public sealed class DuplicateUserException(string email) : Exception(
    $"A user with email '{email}' already exists.");
