using Connect.Domain.Common;
using Connect.Domain.Users;

namespace Connect.Application.Common.Abstractions;

public interface IAppDbContext
{
    IQueryable<Account> Accounts { get; }
    IQueryable<Profile> Profiles { get; }
    void Add<TEntity>(TEntity entity) where TEntity : class, IEntity;
    void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
