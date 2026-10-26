using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Profiles.ListSuggestedTags;

public sealed class ListSuggestedTagsQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<ListSuggestedTagsQuery, Result<SuggestedTagsResult>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<SuggestedTagsResult>> HandleAsync(
        ListSuggestedTagsQuery query,
        CancellationToken cancellationToken)
    {
        var suggested = _dbContext.Tags.AsNoTracking().Where(tag => tag.IsSuggested);

        var total = await suggested.CountAsync(cancellationToken);
        var tags = await suggested
            .OrderBy(tag => tag.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(tag => new TagResult(tag.Id, tag.Name))
            .ToListAsync(cancellationToken);

        return Result<SuggestedTagsResult>.Success(
            new SuggestedTagsResult(tags, query.Page, query.PageSize, total));
    }
}
