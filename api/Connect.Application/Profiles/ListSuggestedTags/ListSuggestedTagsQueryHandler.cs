using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;

namespace Connect.Application.Profiles.ListSuggestedTags;

public sealed class ListSuggestedTagsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<ListSuggestedTagsQuery, Result<SuggestedTagsResult>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<SuggestedTagsResult>> HandleAsync(
        ListSuggestedTagsQuery query,
        CancellationToken cancellationToken)
    {
        var suggested = _dbContext.AsNoTracking(_dbContext.Tags).Where(tag => tag.IsSuggested);

        var total = await _dbContext.CountAsync(suggested, cancellationToken);
        var tags = await _dbContext.ToListAsync(
            suggested
                .OrderBy(tag => tag.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(tag => new TagResult(tag.Id, tag.Name)),
            cancellationToken);

        return Result<SuggestedTagsResult>.Success(
            new SuggestedTagsResult(tags, query.Page, query.PageSize, total));
    }
}
