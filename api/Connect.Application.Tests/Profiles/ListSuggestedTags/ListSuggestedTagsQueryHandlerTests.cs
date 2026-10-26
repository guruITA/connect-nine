using Connect.Application.Profiles.ListSuggestedTags;
using Connect.Application.Tests.Fixtures;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Connect.Application.Tests.Profiles.ListSuggestedTags;

public sealed class ListSuggestedTagsQueryHandlerTests
{
    private SqliteConnection connection;
    private SqliteAppDbContext dbContext;
    private ListSuggestedTagsQueryHandler handler;

    [SetUp]
    public void SetUp()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        dbContext = SqliteAppDbContext.Create(connection);
        dbContext.Database.EnsureCreated();
        handler = new ListSuggestedTagsQueryHandler(dbContext);
        dbContext.SaveTag("photography", isSuggested: true);
        dbContext.SaveTag("football", isSuggested: true);
        dbContext.SaveTag("music", isSuggested: true);
        dbContext.SaveTag("drone video");
    }

    [TearDown]
    public void TearDown()
    {
        dbContext.Dispose();
        connection.Dispose();
    }

    [Test]
    public async Task HandleAsync_FirstPage_ReturnsOnlySuggestedTagsSortedByNameWithTheTotal()
    {
        var result = await handler.HandleAsync(new ListSuggestedTagsQuery(1, 50), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value?.Tags.Select(tag => tag.Name), Is.EqualTo(new[] { "football", "music", "photography" }));
            Assert.That(result.Value?.Total, Is.EqualTo(3));
        }
    }

    [Test]
    public async Task HandleAsync_SecondPageOfTwo_ReturnsTheRest()
    {
        var result = await handler.HandleAsync(new ListSuggestedTagsQuery(2, 2), CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value?.Tags.Select(tag => tag.Name), Is.EqualTo(new[] { "photography" }));
            Assert.That(result.Value?.Total, Is.EqualTo(3));
        }
    }
}
