using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Connect.Application.Profiles.GetPublicProfile;

public sealed class GetPublicProfileQueryHandler(IAppDbContext dbContext)
    : IQueryHandler<GetPublicProfileQuery, Result<GetPublicProfileResponse>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<GetPublicProfileResponse>> HandleAsync(
        GetPublicProfileQuery query,
        CancellationToken cancellationToken)
    {
        // Reads only the four public columns, by the primary key, without tracking.
        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .Where(candidate => candidate.Id == query.ProfileId)
            .Select(candidate => new PublicMemberProfile(
                candidate.FirstName,
                candidate.AgeBand,
                candidate.Region,
                candidate.Bio))
            .SingleOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            return Result<GetPublicProfileResponse>.Failure(
                Error.NotFound("This member does not exist"));
        }

        return Result<GetPublicProfileResponse>.Success(new GetPublicProfileResponse(
            profile.FirstName,
            profile.AgeBand,
            ProfileVisibility.ShowsRegion(profile.AgeBand) ? profile.Region : null,
            ProfileVisibility.ShowsBio(profile.AgeBand) && !string.IsNullOrEmpty(profile.Bio)
                ? profile.Bio
                : null));
    }
}
