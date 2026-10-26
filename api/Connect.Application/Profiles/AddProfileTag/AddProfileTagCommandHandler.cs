using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Connect.Application.Profiles.AddProfileTag;

public sealed class AddProfileTagCommandHandler(IAppDbContext dbContext)
    : ICommandHandler<AddProfileTagCommand, Result<AddProfileTagResult>>
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<Result<AddProfileTagResult>> HandleAsync(
        AddProfileTagCommand command,
        CancellationToken cancellationToken)
    {
        var profileId = await _dbContext.Profiles
            .Where(profile => profile.AccountId == command.AccountId)
            .Select(profile => (Guid?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (profileId is null)
        {
            return Result<AddProfileTagResult>.Failure(Error.NotFound("You have no profile yet"));
        }

        var tag = await GetOrAddTagAsync(command.Name, cancellationToken);
        var result = new TagResult(tag.Id, tag.Name);

        if (await _dbContext.ProfileTags.AnyAsync(
                link => link.ProfileId == profileId && link.TagId == tag.Id,
                cancellationToken))
        {
            return Result<AddProfileTagResult>.Success(new AddProfileTagResult(result, Added: false));
        }

        var link = new ProfileTag { ProfileId = profileId.Value, TagId = tag.Id };
        _dbContext.Add(link);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception, "pk_profile_tag"))
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
        var existing = await _dbContext.Tags.SingleOrDefaultAsync(tag => tag.Name == name, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var tag = new Tag { Name = name, IsSuggested = false };
        _dbContext.Add(tag);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return tag;
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception, "ux_tag_name"))
        {
            _dbContext.Remove(tag);
            return await _dbContext.Tags.SingleAsync(candidate => candidate.Name == name, cancellationToken);
        }
    }

    private static bool IsUniqueViolation(Exception exception, string constraintName)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                } postgresException
                && postgresException.ConstraintName == constraintName)
            {
                return true;
            }
        }

        return false;
    }
}
