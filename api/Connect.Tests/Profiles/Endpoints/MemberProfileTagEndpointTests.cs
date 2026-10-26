using System.Net;
using System.Text.Json;
using Connect.Domain.Tags;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using static Connect.Tests.Fixtures.ProfileTestHelpers;

namespace Connect.Tests.Profiles.Endpoints;

// Runs against a real test database (ConnectionStrings__DefaultConnection).
public sealed class MemberProfileTagEndpointTests
{
    private const string TagsAddress = "/api/member-profiles/current/tags/";
    private const string SuggestedAddress = "/api/tags/suggested";

    [Test]
    public async Task PostTag_NewThenAgain_GivesCreatedThenOkAndOneLink()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, accountId, AgeBand.From18To21);
        using HttpClient client = ClientFor(factory, accountId);
        string name = $"Drone   Video {Guid.NewGuid():N}"[..20];

        using HttpResponseMessage first = await client.PostAsync(TagsAddress, Json(new { name }));
        using HttpResponseMessage second = await client.PostAsync(TagsAddress, Json(new { name = name.ToUpperInvariant() }));
        using JsonDocument body = await ReadJson(first);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.RootElement.GetProperty("data").GetProperty("name").GetString(), Is.EqualTo(TagRules.Normalize(name)));
            Assert.That(LinksOf(factory, profileId), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PostTag_TwoMembersTypingTheSameNewTagAtOnce_ShareOneRow()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid first = SaveAccount(factory);
        Guid second = SaveAccount(factory);
        SaveProfile(factory, first, AgeBand.From18To21);
        SaveProfile(factory, second, AgeBand.From22To30);
        using HttpClient firstClient = ClientFor(factory, first);
        using HttpClient secondClient = ClientFor(factory, second);
        string name = $"shared {Guid.NewGuid():N}"[..20];

        HttpResponseMessage[] responses = await Task.WhenAll(
            firstClient.PostAsync(TagsAddress, Json(new { name })),
            secondClient.PostAsync(TagsAddress, Json(new { name })));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(responses.Select(response => response.StatusCode), Has.All.EqualTo(HttpStatusCode.Created));
            Assert.That(TagsNamed(factory, name), Is.EqualTo(1));
        }

        foreach (HttpResponseMessage response in responses)
        {
            response.Dispose();
        }
    }

    [Test]
    public async Task PostTag_WithoutProfile_ReturnsNotFound()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.PostAsync(TagsAddress, Json(new { name = "football" }));
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("You have no profile yet"));
        }
    }

    [TestCase("", "Fill in a tag")]
    [TestCase("   ", "Fill in a tag")]
    [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "A tag has at most 30 characters")]
    public async Task PostTag_WrongName_ReturnsTheMessageForName(string name, string message)
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        SaveProfile(factory, accountId, AgeBand.From18To21);
        using HttpClient client = ClientFor(factory, accountId);

        using HttpResponseMessage response = await client.PostAsync(TagsAddress, Json(new { name }));
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body.RootElement.GetProperty("errors").GetProperty("name")[0].GetString(), Is.EqualTo(message));
        }
    }

    [Test]
    public async Task EveryAddress_WithoutAccount_ReturnsUnauthorized()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage post = await client.PostAsync(TagsAddress, Json(new { name = "football" }));
        using HttpResponseMessage suggested = await client.GetAsync(SuggestedAddress);
        using HttpResponseMessage current = await client.GetAsync("/api/member-profiles/current");

        Assert.That(
            new[] { post.StatusCode, suggested.StatusCode, current.StatusCode },
            Has.All.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetSuggested_ReturnsTheStarterListSortedWithTheTotal()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"{SuggestedAddress}?page=1&pageSize=50");
        using JsonDocument body = await ReadJson(response);
        List<string> names = body.RootElement.GetProperty("data").EnumerateArray()
            .Select(tag => tag.GetProperty("name").GetString()
                ?? throw new AssertionException("Expected every tag to have a name."))
            .ToList();
        JsonElement meta = body.RootElement.GetProperty("meta");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(names, Is.Ordered);
            Assert.That(names, Does.Contain("football"));
            Assert.That(meta.GetProperty("total").GetInt32(), Is.EqualTo(names.Count));
            Assert.That(meta.GetProperty("page").GetInt32(), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task GetSuggested_WrongPage_ReturnsBadRequest()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        using HttpClient client = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"{SuggestedAddress}?page=abc");
        using JsonDocument body = await ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("Use a page number of 1 or more"));
        }
    }

    [Test]
    public async Task GetCurrent_AfterAddingTags_HoldsTheTagsSortedByName()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid accountId = SaveAccount(factory);
        SaveProfile(factory, accountId, AgeBand.From18To21);
        using HttpClient client = ClientFor(factory, accountId);
        using HttpResponseMessage photography = await client.PostAsync(TagsAddress, Json(new { name = "photography" }));
        using HttpResponseMessage football = await client.PostAsync(TagsAddress, Json(new { name = "Football" }));

        using HttpResponseMessage response = await client.GetAsync("/api/member-profiles/current");
        using JsonDocument body = await ReadJson(response);

        Assert.That(
            body.RootElement.GetProperty("data").GetProperty("tags").EnumerateArray().Select(tag => tag.GetProperty("name").GetString()),
            Is.EqualTo(new[] { "football", "photography" }));
    }

    [Test]
    public async Task PostTag_DatabaseNotReachable_ReturnsServiceUnavailableWithoutDatabaseText()
    {
        using WebApplicationFactory<Program> factory = UnreachableDatabaseFactory();
        using HttpClient client = ClientFor(factory, Guid.NewGuid());

        using HttpResponseMessage response = await client.PostAsync(TagsAddress, Json(new { name = "football" }));
        string content = await response.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
            Assert.That(content, Does.Not.Contain("Exception").And.Not.Contain("football"));
        }
    }

    private static int LinksOf(WebApplicationFactory<Program> factory, Guid profileId)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ConnectDbContext>()
            .Set<ProfileTag>().Count(link => link.ProfileId == profileId);
    }

    private static int TagsNamed(WebApplicationFactory<Program> factory, string name)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ConnectDbContext>()
            .Set<Tag>().Count(tag => tag.Name == name);
    }
}
