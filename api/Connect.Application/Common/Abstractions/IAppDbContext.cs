using Connect.Domain.Users;

namespace Connect.Application.Common.Abstractions;

public interface IAppDbContext
{
    IQueryable<Account> Accounts { get; }
    IQueryable<Profile> Profiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
