using Connect.Application.Common.Results;
using Connect.Domain.Common;
using Connect.Domain.Tags;
using Connect.Domain.Users;

namespace Connect.Application.Common.Abstractions;

public interface IAppDbContext
{
    IQueryable<Account> Accounts { get; }
    IQueryable<Profile> Profiles { get; }
    IQueryable<Tag> Tags { get; }
    IQueryable<ProfileTag> ProfileTags { get; }
    void Add<TEntity>(TEntity entity) where TEntity : class, IEntity;
    void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<Result> SaveChangesOrConflictAsync(CancellationToken cancellationToken);
    Task<TEntity?> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken);
    Task<TEntity> SingleAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken);
    Task<bool> AnyAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken);
    Task<int> CountAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken);
    Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken);
    IQueryable<TEntity> AsNoTracking<TEntity>(IQueryable<TEntity> query) where TEntity : class;
}
