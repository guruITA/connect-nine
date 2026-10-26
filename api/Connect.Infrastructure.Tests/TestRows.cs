using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;

namespace Connect.Infrastructure.Tests;

// The rows the database tests need (backend rule 20). What a test checks stays in the test.
internal static class TestRows
{
    public static Account SaveAccount(ConnectDbContext dbContext)
    {
        string email = $"profile-{Guid.NewGuid():N}@example.com";
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant()
        };

        dbContext.Add(account);
        dbContext.SaveChanges();

        return account;
    }

    public static Profile NewProfile(Account account, string firstName = "Sam")
    {
        return new Profile
        {
            AccountId = account.Id,
            Account = account,
            FirstName = firstName,
            AgeBand = AgeBand.Adolescent,
            Region = "Amstelveen",
            Bio = "I like football."
        };
    }
}
