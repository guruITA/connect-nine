using System.Data.Common;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.GetCurrentProfile;
using Connect.Application.Tests.Fixtures;
using Connect.Domain.Users;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.GetCurrentProfile;

public sealed class GetCurrentProfileQueryHandlerTests
{
    private SqliteConnection connection;
    private SqliteAppDbContext dbContext;
    private GetCurrentProfileQueryHandler handler;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        dbContext = SqliteAppDbContext.Create(connection);
        dbContext.Database.EnsureCreated();
        handler = new GetCurrentProfileQueryHandler(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_AccountWithProfile_ReturnsTheFourValues()
    {
        Account account = dbContext.SaveAccount();
        dbContext.SaveProfile(account, "Sam", AgeBand.YoungAdult, "Amstelveen", "I like football.");

        var result = await handler.HandleAsync(new GetCurrentProfileQuery(account.Id), CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(
            result.Value,
            Is.EqualTo(new CurrentProfileResult("Sam", AgeBand.YoungAdult, "Amstelveen", "I like football.")));
    }

    [Test]
    public async Task HandleAsync_AccountWithoutProfile_ReturnsNotFound()
    {
        Account account = dbContext.SaveAccount();

        var result = await handler.HandleAsync(new GetCurrentProfileQuery(account.Id), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.NotFound));
            Assert.That(result.Error.Description, Is.EqualTo("You have no profile yet"));
        }
    }

    [Test]
    public async Task HandleAsync_TwoProfiles_ReturnsOnlyTheOwnOne()
    {
        Account own = dbContext.SaveAccount();
        Account other = dbContext.SaveAccount();
        dbContext.SaveProfile(own, "Sam", AgeBand.Adolescent, "Amstelveen", null);
        dbContext.SaveProfile(other, "Alex", AgeBand.Adult, "Uithoorn", "Other text");

        var result = await handler.HandleAsync(new GetCurrentProfileQuery(own.Id), CancellationToken.None);

        Assert.That(result.Value?.FirstName, Is.EqualTo("Sam"));
    }

    [Test]
    public void HandleAsync_DatabaseNotReachable_ThrowsTheDatabaseException()
    {
        using var unreachable = new SqliteConnection("Data Source=/nonexistent/connect.db;Mode=ReadOnly");
        using SqliteAppDbContext unreachableContext = SqliteAppDbContext.Create(unreachable);
        var unreachableHandler = new GetCurrentProfileQueryHandler(unreachableContext);

        Assert.ThrowsAsync(
            Is.InstanceOf<DbException>(),
            () => unreachableHandler.HandleAsync(new GetCurrentProfileQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
