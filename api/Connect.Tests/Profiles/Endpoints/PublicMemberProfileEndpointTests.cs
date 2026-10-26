using System.Net;
using System.Text.Json;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using static Connect.Tests.Fixtures.ProfileTestHelpers;

namespace Connect.Tests.Profiles.Endpoints;

// The tags on the page of another member (task 36c.2), against a real test database.
public sealed class PublicMemberProfileEndpointTests
{
    private const string TagsAddress = "/api/member-profiles/current/tags/";

    [Test]
    public async Task GetById_MemberWithTags_GivesTheOtherMemberTheNamesSortedWithoutIds()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid memberA = SaveAccount(factory);
        Guid profileA = SaveProfile(factory, memberA, AgeBand.YoungAdult);
        using HttpClient clientA = ClientFor(factory, memberA);
        using HttpResponseMessage photography = await clientA.PostAsync(TagsAddress, Json(new { name = "photography" }));
        using HttpResponseMessage football = await clientA.PostAsync(TagsAddress, Json(new { name = "football" }));
        using HttpClient clientB = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await clientB.GetAsync($"/api/member-profiles/{profileA}");
        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument body = JsonDocument.Parse(content);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(
                body.RootElement.GetProperty("data").GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()),
                Is.EqualTo(new[] { "football", "photography" }));
            Assert.That(content, Does.Not.Contain("\"id\""));
        }
    }

    [Test]
    public async Task GetById_YoungPersonOf15To17_ShowsTheTagsLikeTheShortText()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid minor = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, minor, AgeBand.Adolescent);
        using HttpClient minorClient = ClientFor(factory, minor);
        using HttpResponseMessage dance = await minorClient.PostAsync(TagsAddress, Json(new { name = "dance" }));
        using HttpClient other = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await other.GetAsync($"/api/member-profiles/{profileId}");
        using JsonDocument body = await ReadJson(response);
        JsonElement data = body.RootElement.GetProperty("data");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(data.TryGetProperty("region", out _), Is.False);
            Assert.That(data.GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()), Is.EqualTo(new[] { "dance" }));
        }
    }

    [Test]
    public async Task GetById_WithoutAccount_StillReturnsUnauthorized()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid profileId = SaveProfile(factory, SaveAccount(factory), AgeBand.YoungAdult);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetById_DatabaseNotReachable_StillReturnsServiceUnavailable()
    {
        using WebApplicationFactory<Program> factory = UnreachableDatabaseFactory();
        using HttpClient client = ClientFor(factory, Guid.NewGuid());

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
    }
}
