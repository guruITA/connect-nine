using System.Net;
using System.Text.Json;
using Connect.Domain.Users;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Connect.Tests.Profiles.Endpoints;

// Next to MemberProfileEndpointTests.cs: the visitor cases of #348. Until log in (#15) a visitor
// is a request without the header X-Temporary-Account-Id, and the guard answers 401 without a
// body before the handler runs, so there is no ProblemDetails yet and no traceId to leave out.
public sealed class VisitorMemberProfileEndpointTests
{
    private const string UnknownId = "00000000-0000-4000-8000-000000000999";
    private const string NotAnId = "abc";

    [Test]
    public async Task Visitor_OpeningKnownUnknownOrMalformedId_GetsExactlyTheSameRefusal()
    {
        using WebApplicationFactory<Program> factory = ProfileTestHelpers.CreateFactory();
        Guid accountId = ProfileTestHelpers.SaveAccount(factory);
        Guid profileId = ProfileTestHelpers.SaveProfile(factory, accountId, AgeBand.YoungAdult);
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage known = await client.GetAsync($"/api/member-profiles/{profileId}");
        using HttpResponseMessage unknown = await client.GetAsync($"/api/member-profiles/{UnknownId}");
        using HttpResponseMessage malformed = await client.GetAsync($"/api/member-profiles/{NotAnId}");
        string knownBody = await known.Content.ReadAsStringAsync();
        string unknownBody = await unknown.Content.ReadAsStringAsync();
        string malformedBody = await malformed.Content.ReadAsStringAsync();

        using (Assert.EnterMultipleScope())
        {
            foreach (HttpResponseMessage response in new[] { known, unknown, malformed })
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(response.Content.Headers.ContentType, Is.Null);
            }

            Assert.That(knownBody, Is.Empty);
            Assert.That(unknownBody, Is.EqualTo(knownBody));
            Assert.That(malformedBody, Is.EqualTo(knownBody));
        }
    }

    [Test]
    public async Task LoggedInMember_OpeningTheKnownId_ReturnsOk()
    {
        using WebApplicationFactory<Program> factory = ProfileTestHelpers.CreateFactory();
        Guid ownerAccountId = ProfileTestHelpers.SaveAccount(factory);
        Guid profileId = ProfileTestHelpers.SaveProfile(factory, ownerAccountId, AgeBand.YoungAdult);
        using HttpClient client = ProfileTestHelpers.ClientFor(factory, ProfileTestHelpers.SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCase(UnknownId)]
    [TestCase(NotAnId)]
    public async Task LoggedInMember_OpeningUnknownOrMalformedId_ReturnsNotFoundWithTheSameMessage(string id)
    {
        using WebApplicationFactory<Program> factory = ProfileTestHelpers.CreateFactory();
        using HttpClient client = ProfileTestHelpers.ClientFor(factory, ProfileTestHelpers.SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{id}");
        using JsonDocument body = await ProfileTestHelpers.ReadJson(response);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(body.RootElement.GetProperty("title").GetString(), Is.EqualTo("This member does not exist"));
        }
    }
}
