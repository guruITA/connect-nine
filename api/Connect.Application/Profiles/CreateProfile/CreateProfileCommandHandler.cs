using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Users;

namespace Connect.Application.Profiles.CreateProfile;

public sealed class CreateProfileCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<CreateProfileCommand, Result<CreateProfileResult>>
{
    private const string AlreadyHasProfile = "You already have a profile";

    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<CreateProfileResult>> HandleAsync(
        CreateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var account = await _dbContext.SingleOrDefaultAsync(
            _dbContext.Accounts.Where(candidate => candidate.Id == command.AccountId),
            cancellationToken);
        if (account is null)
        {
            return Result<CreateProfileResult>.Failure(
                Error.Unauthorized("Log in first to save your profile"));
        }

        if (await _dbContext.AnyAsync(
                _dbContext.Profiles.Where(profile => profile.AccountId == command.AccountId),
                cancellationToken))
        {
            return Result<CreateProfileResult>.Failure(Error.Conflict(AlreadyHasProfile));
        }

        var profile = new Profile
        {
            AccountId = account.Id,
            Account = account,
            FirstName = command.FirstName,
            AgeBand = command.AgeBand,
            Region = command.Region,
            Bio = command.Bio
        };
        _dbContext.Add(profile);

        var saved = await _dbContext.SaveChangesOrConflictAsync(cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<CreateProfileResult>.Failure(Error.Conflict(AlreadyHasProfile));
        }

        return Result<CreateProfileResult>.Success(new CreateProfileResult(profile.Id));
    }
}
