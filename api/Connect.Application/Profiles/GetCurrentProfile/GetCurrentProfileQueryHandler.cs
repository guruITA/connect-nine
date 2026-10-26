using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Profiles.GetCurrentProfile;

public sealed class GetCurrentProfileQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetCurrentProfileQuery, Result<CurrentProfileResult>>
{
    public async Task<Result<CurrentProfileResult>> HandleAsync(
        GetCurrentProfileQuery query,
        CancellationToken cancellationToken)
    {
        // Found through the unique index on account_id, and never loads the account itself.
        var profile = await dbContext.Profiles
            .AsNoTracking()
            .Where(candidate => candidate.AccountId == query.AccountId)
            .Select(candidate => new CurrentProfileResult(
                candidate.FirstName,
                candidate.AgeBand,
                candidate.Region,
                candidate.Bio))
            .SingleOrDefaultAsync(cancellationToken);

        return profile is null
            ? Result<CurrentProfileResult>.Failure(Error.NotFound("You have no profile yet"))
            : Result<CurrentProfileResult>.Success(profile);
    }
}
