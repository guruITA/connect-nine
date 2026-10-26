using System.Data.Common;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.CreateProfile;
using Connect.Application.Tests.Fixtures;
using Connect.Domain.Users;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.CreateProfile;

public sealed class CreateProfileHandlerTests
{
    private SqliteConnection connection;
    private SqliteAppDbContext dbContext;
    private CreateProfileCommandHandler handler;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        dbContext = SqliteAppDbContext.Create(connection);
        dbContext.Database.EnsureCreated();
        handler = new CreateProfileCommandHandler(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_FirstProfile_SavesItAndReturnsItsId()
    {
        Account account = dbContext.SaveAccount();

        var result = await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Sam", AgeBand.YoungAdult, "Amstelveen", null),
            CancellationToken.None);

        Profile saved = dbContext.Profiles.Single();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value?.Id, Is.EqualTo(saved.Id));
            Assert.That(saved.AccountId, Is.EqualTo(account.Id));
            Assert.That(saved.FirstName, Is.EqualTo("Sam"));
            Assert.That(saved.AgeBand, Is.EqualTo(AgeBand.YoungAdult));
            Assert.That(saved.Region, Is.EqualTo("Amstelveen"));
            Assert.That(saved.Bio, Is.Null);
        }
    }

    [Test]
    public async Task HandleAsync_SecondProfileForSameAccount_ReturnsConflictAndSavesNothing()
    {
        Account account = dbContext.SaveAccount();
        await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Sam", AgeBand.Adolescent, "Amstelveen", null),
            CancellationToken.None);

        var result = await handler.HandleAsync(
            new CreateProfileCommand(account.Id, "Alex", AgeBand.Adult, "Uithoorn", "Hi"),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.Conflict));
            Assert.That(result.Error.Description, Is.EqualTo("You already have a profile"));
            Assert.That(dbContext.Profiles.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task HandleAsync_UnknownAccount_ReturnsUnauthorizedAndSavesNothing()
    {
        var result = await handler.HandleAsync(
            new CreateProfileCommand(Guid.NewGuid(), "Sam", AgeBand.Adolescent, "Amstelveen", null),
            CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.Unauthorized));
            Assert.That(dbContext.Profiles.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void HandleAsync_DatabaseNotReachable_ThrowsTheDatabaseException()
    {
        using var unreachable = new SqliteConnection("Data Source=/nonexistent/connect.db;Mode=ReadOnly");
        using SqliteAppDbContext unreachableContext = SqliteAppDbContext.Create(unreachable);
        var unreachableHandler = new CreateProfileCommandHandler(unreachableContext);

        Assert.ThrowsAsync(
            Is.InstanceOf<DbException>(),
            () => unreachableHandler.HandleAsync(
                new CreateProfileCommand(Guid.NewGuid(), "Sam", AgeBand.Adolescent, "Amstelveen", null),
                CancellationToken.None));
    }
}
