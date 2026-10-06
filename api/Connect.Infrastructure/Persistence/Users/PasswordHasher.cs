using Connect.Application.Users.Contracts;
using Connect.Domain.Users;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<Connect.Domain.Users.User>;

namespace Connect.Infrastructure.Persistence.Users;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly IdentityPasswordHasher _passwordHasher = new();

    public string Hash(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }
}
