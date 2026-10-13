using Connect.Domain.Users;

namespace Connect.Application.Common.Abstractions;

public interface IAppDbContext
{
    IQueryable<Account> Accounts { get; }
    IQueryable<Profile> Profiles { get; }
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
