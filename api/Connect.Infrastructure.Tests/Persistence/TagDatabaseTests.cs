using Connect.Domain.Tags;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

public sealed class TagDatabaseTests
{
    private string connectionString;
    private ConnectDbContext dbContext;

    [SetUp]
    public void SetUp()
    {
        connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DefaultConnection is not configured.");

        dbContext = CreateDbContext();
        dbContext.MigrateAndReloadTypes();
    }

    [TearDown]
    public void TearDown()
    {
        dbContext?.Dispose();
    }

    [Test]
    public void SavingANameThatOnlyDiffersInCapitals_FailsOnTheUniqueName()
    {
        string name = $"tag {Guid.NewGuid():N}"[..20];
        dbContext.Add(new Tag { Name = name });
        dbContext.SaveChanges();

        using ConnectDbContext secondContext = CreateDbContext();
        secondContext.Add(new Tag { Name = name.ToUpperInvariant() });

        PostgresException exception = FindPostgresException(Assert.Throws<DbUpdateException>(() => secondContext.SaveChanges()));

        Assert.That(
            (exception.SqlState, exception.ConstraintName),
            Is.EqualTo((PostgresErrorCodes.UniqueViolation, "ux_tag_name")));
    }

    [Test]
    public void SavingANameOf31Characters_FailsOnTheLengthCheck()
    {
        dbContext.Add(new Tag { Name = new string('a', 31) });

        PostgresException exception = FindPostgresException(Assert.Throws<DbUpdateException>(() => dbContext.SaveChanges()));

        Assert.That(
            (exception.SqlState, exception.ConstraintName),
            Is.EqualTo((PostgresErrorCodes.CheckViolation, "ck_tag_name_length")));
    }

    [Test]
    public void TheStarterList_HasTwentySuggestedTagsInSmallLetters()
    {
        List<string> names = dbContext.Set<Tag>()
            .Where(tag => tag.IsSuggested)
            .OrderBy(tag => tag.Name)
            .Select(tag => tag.Name)
            .ToList();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(names, Has.Count.EqualTo(20));
            Assert.That(names, Is.Ordered);
            Assert.That(names.All(name => name == name.ToLowerInvariant()), Is.True);
            Assert.That(names, Does.Contain("football").And.Contain("photography").And.Contain("dance"));
        }
    }

    [Test]
    public void LinkingATagTwice_FailsOnThePrimaryKeyOfTheLink()
    {
        Profile profile = SaveProfile();
        Tag tag = SaveTag();
        dbContext.Add(new ProfileTag { ProfileId = profile.Id, TagId = tag.Id });
        dbContext.SaveChanges();

        using ConnectDbContext secondContext = CreateDbContext();
        secondContext.Add(new ProfileTag { ProfileId = profile.Id, TagId = tag.Id });

        PostgresException exception = FindPostgresException(Assert.Throws<DbUpdateException>(() => secondContext.SaveChanges()));

        Assert.That(
            (exception.SqlState, exception.ConstraintName),
            Is.EqualTo((PostgresErrorCodes.UniqueViolation, "pk_profile_tag")));
    }

    [Test]
    public void DeletingATagThatAProfileUses_IsRefused()
    {
        Profile profile = SaveProfile();
        Tag tag = SaveTag();
        dbContext.Add(new ProfileTag { ProfileId = profile.Id, TagId = tag.Id });
        dbContext.SaveChanges();

        PostgresException exception = Assert.Throws<PostgresException>(
            () => dbContext.Database.ExecuteSql($"DELETE FROM tag WHERE id = {tag.Id}")) ?? throw new AssertionException("Expected PostgreSQL to reject the write.");

        Assert.That(exception.SqlState, Is.EqualTo(PostgresErrorCodes.ForeignKeyViolation));
    }

    [Test]
    public void DeletingAProfile_RemovesItsLinksAndKeepsTheTags()
    {
        Profile profile = SaveProfile();
        Tag tag = SaveTag();
        dbContext.Add(new ProfileTag { ProfileId = profile.Id, TagId = tag.Id });
        dbContext.SaveChanges();

        dbContext.Database.ExecuteSql($"DELETE FROM profile WHERE id = {profile.Id}");

        using ConnectDbContext readContext = CreateDbContext();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(readContext.Set<ProfileTag>().Count(link => link.ProfileId == profile.Id), Is.EqualTo(0));
            Assert.That(readContext.Set<Tag>().Count(candidate => candidate.Id == tag.Id), Is.EqualTo(1));
        }
    }

    private ConnectDbContext CreateDbContext()
    {
        return new ConnectDbContext(new DbContextOptionsBuilder<ConnectDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options);
    }

    private Tag SaveTag()
    {
        var tag = new Tag { Name = $"t{Guid.NewGuid():N}"[..20] };
        dbContext.Add(tag);
        dbContext.SaveChanges();
        return tag;
    }

    private Profile SaveProfile()
    {
        string email = $"tags-{Guid.NewGuid():N}@example.com";
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant()
        };
        var profile = new Profile
        {
            AccountId = account.Id,
            Account = account,
            FirstName = "Sam",
            AgeBand = AgeBand.From18To21,
            Region = "Amstelveen"
        };

        dbContext.Add(profile);
        dbContext.SaveChanges();
        return profile;
    }

    private static PostgresException FindPostgresException(DbUpdateException? exception)
    {
        return exception?.InnerException as PostgresException
            ?? throw new AssertionException("Expected PostgreSQL to reject the write.");
    }
}
