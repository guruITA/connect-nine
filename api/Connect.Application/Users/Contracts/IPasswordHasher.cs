using Connect.Domain.Users;

namespace Connect.Application.Users.Contracts;

public interface IPasswordHasher
{
    string Hash(User user, string password);
}
