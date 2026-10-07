namespace Connect.Application.Users.DTO;

public sealed class DuplicateUserException(string email) : Exception(
    $"A user with email '{email}' already exists.");
