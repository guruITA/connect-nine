using Connect.Domain.Authentication;

namespace Connect.Application.Authentication.Users;

public interface IPasswordHasher
{
    string Hash(User user, string password);
}
