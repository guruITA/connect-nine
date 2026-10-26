using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Tags;
using Connect.Domain.Users;

namespace Connect.Application.Profiles.AddProfileTag;

public sealed class AddProfileTagCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<AddProfileTagCommand, Result<AddProfileTagResult>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<AddProfileTagResult>> HandleAsync(
        AddProfileTagCommand command,
        CancellationToken cancellationToken)
    {
        var profileId = await _dbContext.SingleOrDefaultAsync(
            _dbContext.Profiles
                .Where(profile => profile.AccountId == command.AccountId)
                .Select(profile => (Guid?)profile.Id),
            cancellationToken);
        if (profileId is null)
        {
            return Result<AddProfileTagResult>.Failure(Error.NotFound("You have no profile yet"));
        }

        var tag = await GetOrAddTagAsync(command.Name, cancellationToken);
        var result = new TagResult(tag.Id, tag.Name);

        if (await _dbContext.AnyAsync(
                _dbContext.ProfileTags.Where(link => link.ProfileId == profileId && link.TagId == tag.Id),
                cancellationToken))
        {
            return Result<AddProfileTagResult>.Success(new AddProfileTagResult(result, Added: false));
        }

        // Two saves of the same link at once both end with the one row: the second hits
        // ux_profile_tag_profile_id_tag_id and answers that the tag was already there.
        var link = new ProfileTag { ProfileId = profileId.Value, TagId = tag.Id };
        _dbContext.Add(link);
        var saved = await _dbContext.SaveChangesOrConflictAsync(cancellationToken);
        if (!saved.IsSuccess)
        {
            _dbContext.Remove(link);
            return Result<AddProfileTagResult>.Success(new AddProfileTagResult(result, Added: false));
        }

        return Result<AddProfileTagResult>.Success(new AddProfileTagResult(result, Added: true));
    }

    // Two members typing the same new tag at once both get the one row: the second insert hits
    // ux_tag_name and reads the tag the first one made.
    private async Task<Tag> GetOrAddTagAsync(string name, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.SingleOrDefaultAsync(
            _dbContext.Tags.Where(tag => tag.Name == name),
            cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var tag = new Tag { Name = name, IsSuggested = false };
        _dbContext.Add(tag);
        var saved = await _dbContext.SaveChangesOrConflictAsync(cancellationToken);
        if (saved.IsSuccess)
        {
            return tag;
        }

        _dbContext.Remove(tag);
        return await _dbContext.SingleAsync(
            _dbContext.Tags.Where(candidate => candidate.Name == name),
            cancellationToken);
    }
}
