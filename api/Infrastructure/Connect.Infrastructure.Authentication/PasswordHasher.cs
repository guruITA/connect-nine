using Connect.Application.Authentication.Users;
using Connect.Domain.Authentication;
using Microsoft.AspNetCore.Identity;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<Connect.Domain.Authentication.User>;

namespace Connect.Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly IdentityPasswordHasher _passwordHasher = new();

    public string Hash(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }
}
