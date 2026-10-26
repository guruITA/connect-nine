using System.Data.Common;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.AddProfileTag;
using Connect.Application.Tests.Fixtures;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.AddProfileTag;

public sealed class AddProfileTagCommandHandlerTests
{
    private SqliteConnection connection = null!;
    private SqliteAppDbContext dbContext = null!;
    private AddProfileTagCommandHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        dbContext = SqliteAppDbContext.Create(connection);
        dbContext.Database.EnsureCreated();
        handler = new AddProfileTagCommandHandler(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_ExistingTag_IsReusedAndLinked()
    {
        Account account = dbContext.SaveAccount();
        Profile profile = dbContext.SaveProfile(account, "Sam", AgeBand.From18To21, "Amstelveen", null);
        Tag football = dbContext.SaveTag("football", isSuggested: true);

        var result = await handler.HandleAsync(new AddProfileTagCommand(account.Id, "football"), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value!.Added, Is.True);
            Assert.That(result.Value.Tag.Id, Is.EqualTo(football.Id));
            Assert.That(dbContext.Tags.Count(), Is.EqualTo(1));
            Assert.That(dbContext.ProfileTags.Single().ProfileId, Is.EqualTo(profile.Id));
        }
    }

    [Test]
    public async Task HandleAsync_NewTag_IsMadeAsNotSuggested()
    {
        Account account = dbContext.SaveAccount();
        dbContext.SaveProfile(account, "Sam", AgeBand.From18To21, "Amstelveen", null);

        var result = await handler.HandleAsync(new AddProfileTagCommand(account.Id, "drone video"), CancellationToken.None);

        Tag made = dbContext.Tags.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value!.Added, Is.True);
            Assert.That(made.Name, Is.EqualTo("drone video"));
            Assert.That(made.IsSuggested, Is.False);
        }
    }

    [Test]
    public async Task HandleAsync_TagTheProfileAlreadyHas_ChangesNothing()
    {
        Account account = dbContext.SaveAccount();
        dbContext.SaveProfile(account, "Sam", AgeBand.From18To21, "Amstelveen", null);
        await handler.HandleAsync(new AddProfileTagCommand(account.Id, "football"), CancellationToken.None);

        var result = await handler.HandleAsync(new AddProfileTagCommand(account.Id, "football"), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value!.Added, Is.False);
            Assert.That(dbContext.ProfileTags.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task HandleAsync_TwoMembersAddingTheSameTag_ShareOneTag()
    {
        Account first = dbContext.SaveAccount();
        Account second = dbContext.SaveAccount();
        dbContext.SaveProfile(first, "Sam", AgeBand.From18To21, "Amstelveen", null);
        dbContext.SaveProfile(second, "Alex", AgeBand.From22To30, "Uithoorn", null);

        await handler.HandleAsync(new AddProfileTagCommand(first.Id, "drone video"), CancellationToken.None);
        await handler.HandleAsync(new AddProfileTagCommand(second.Id, "drone video"), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dbContext.Tags.Count(), Is.EqualTo(1));
            Assert.That(dbContext.ProfileTags.Count(), Is.EqualTo(2));
        }
    }

    [Test]
    public async Task HandleAsync_AccountWithoutProfile_ReturnsNotFound()
    {
        Account account = dbContext.SaveAccount();

        var result = await handler.HandleAsync(new AddProfileTagCommand(account.Id, "football"), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.NotFound));
            Assert.That(result.Error.Description, Is.EqualTo("You have no profile yet"));
            Assert.That(dbContext.Tags.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void HandleAsync_DatabaseNotReachable_ThrowsTheDatabaseException()
    {
        using var unreachable = new SqliteConnection("Data Source=/nonexistent/connect.db;Mode=ReadOnly");
        using SqliteAppDbContext unreachableContext = SqliteAppDbContext.Create(unreachable);
        var unreachableHandler = new AddProfileTagCommandHandler(unreachableContext);

        Assert.ThrowsAsync(
            Is.InstanceOf<DbException>(),
            () => unreachableHandler.HandleAsync(new AddProfileTagCommand(Guid.NewGuid(), "football"), CancellationToken.None));
    }
}
