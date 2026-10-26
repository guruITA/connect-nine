using System.Data.Common;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.GetPublicProfile;
using Connect.Application.Tests.Fixtures;
using Connect.Domain.Users;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.GetPublicProfile;

public sealed class GetPublicProfileQueryHandlerTests
{
    private SqliteConnection connection = null!;
    private SqliteAppDbContext dbContext = null!;
    private GetPublicProfileQueryHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        dbContext = SqliteAppDbContext.Create(connection);
        dbContext.Database.EnsureCreated();
        handler = new GetPublicProfileQueryHandler(dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_Adult_ReturnsTheFourValues()
    {
        Profile profile = dbContext.SaveProfile(dbContext.SaveAccount(), "Robin", AgeBand.From18To21, "Amstelveen", "Test member.");

        var result = await handler.HandleAsync(new GetPublicProfileQuery(profile.Id), CancellationToken.None);

        Assert.That(
            result.Value,
            Is.EqualTo(new GetPublicProfileResponse("Robin", AgeBand.From18To21, "Amstelveen", "Test member.")));
    }

    [Test]
    public async Task HandleAsync_YoungPersonOf15To17_LeavesTheRegionOut()
    {
        Profile profile = dbContext.SaveProfile(dbContext.SaveAccount(), "Noor", AgeBand.From15To17, "Uithoorn", "Test member.");

        var result = await handler.HandleAsync(new GetPublicProfileQuery(profile.Id), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value!.Region, Is.Null);
            Assert.That(result.Value.Bio, Is.EqualTo("Test member."));
        }
    }

    [Test]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var result = await handler.HandleAsync(new GetPublicProfileQuery(Guid.NewGuid()), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.NotFound));
            Assert.That(result.Error.Description, Is.EqualTo("This member does not exist"));
        }
    }

    [Test]
    public void HandleAsync_DatabaseNotReachable_ThrowsTheDatabaseException()
    {
        using var unreachable = new SqliteConnection("Data Source=/nonexistent/connect.db;Mode=ReadOnly");
        using SqliteAppDbContext unreachableContext = SqliteAppDbContext.Create(unreachable);
        var unreachableHandler = new GetPublicProfileQueryHandler(unreachableContext);

        Assert.ThrowsAsync(
            Is.InstanceOf<DbException>(),
            () => unreachableHandler.HandleAsync(new GetPublicProfileQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
