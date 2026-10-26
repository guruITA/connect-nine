using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;

namespace Connect.Application.Profiles.GetCurrentProfile;

public sealed class GetCurrentProfileQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetCurrentProfileQuery, Result<CurrentProfileResult>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<CurrentProfileResult>> HandleAsync(
        GetCurrentProfileQuery query,
        CancellationToken cancellationToken)
    {
        // Found through the unique index on account_id, without tracking, and never loads the
        // account itself.
        var profile = await _dbContext.SingleOrDefaultAsync(
            _dbContext.AsNoTracking(_dbContext.Profiles)
                .Where(candidate => candidate.AccountId == query.AccountId)
                .Select(candidate => new CurrentProfileResult(
                    candidate.FirstName,
                    candidate.AgeBand,
                    candidate.Region,
                    candidate.Bio)),
            cancellationToken);

        return profile is null
            ? Result<CurrentProfileResult>.Failure(Error.NotFound("You have no profile yet"))
            : Result<CurrentProfileResult>.Success(profile);
    }
}
