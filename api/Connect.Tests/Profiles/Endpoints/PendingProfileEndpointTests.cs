using System.Net;
using System.Text.Json;
using Connect.Domain.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using static Connect.Tests.Fixtures.ProfileTestHelpers;

namespace Connect.Tests.Profiles.Endpoints;

// A profile that waits for a parent (#392) stays out of sight on both addresses (#395), against
// a real test database. The seed of #393 does not exist yet, so every test makes its own
// pending profile.
public sealed class PendingProfileEndpointTests
{
    private const string CurrentAddress = "/api/member-profiles/current";

    [Test]
    public async Task GetById_PendingProfileViewedByAnotherMember_GivesTheUnknownIdAnswer()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveParentConsent(factory, profileId);
        using HttpClient stranger = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage pendingResponse = await stranger.GetAsync($"/api/member-profiles/{profileId}");
        using HttpResponseMessage unknownResponse = await stranger.GetAsync($"/api/member-profiles/{Guid.NewGuid()}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pendingResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                await NormalizedProblemAsync(pendingResponse),
                Is.EqualTo(await NormalizedProblemAsync(unknownResponse)));
        }
    }

    [Test]
    public async Task GetById_PendingProfileViewedByItsOwner_ReturnsOk()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveParentConsent(factory, profileId);
        using HttpClient ownerClient = ClientFor(factory, owner);

        using HttpResponseMessage response = await ownerClient.GetAsync($"/api/member-profiles/{profileId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetById_ProfileWithPermissionGiven_ReturnsOkForAnotherMember()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.YoungAdult);
        SaveParentConsent(factory, profileId, givenAt: DateTimeOffset.UtcNow);
        using HttpClient stranger = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage response = await stranger.GetAsync($"/api/member-profiles/{profileId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCurrent_PendingParentConsent_ShowsPending()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveParentConsent(factory, profileId);
        using HttpClient ownerClient = ClientFor(factory, owner);

        using HttpResponseMessage response = await ownerClient.GetAsync(CurrentAddress);
        using JsonDocument body = await ReadJson(response);

        Assert.That(
            body.RootElement.GetProperty("data").GetProperty("parentConsent").GetString(),
            Is.EqualTo("pending"));
    }

    [Test]
    public async Task GetCurrent_WithoutARequest_LeavesTheKeyOut()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        SaveProfile(factory, owner, AgeBand.YoungAdult);
        using HttpClient ownerClient = ClientFor(factory, owner);

        using HttpResponseMessage response = await ownerClient.GetAsync(CurrentAddress);
        using JsonDocument body = await ReadJson(response);

        Assert.That(body.RootElement.GetProperty("data").TryGetProperty("parentConsent", out _), Is.False);
    }

    [Test]
    public async Task GetCurrent_PermissionGiven_LeavesTheKeyOut()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveParentConsent(factory, profileId, givenAt: DateTimeOffset.UtcNow);
        using HttpClient ownerClient = ClientFor(factory, owner);

        using HttpResponseMessage response = await ownerClient.GetAsync(CurrentAddress);
        using JsonDocument body = await ReadJson(response);

        Assert.That(body.RootElement.GetProperty("data").TryGetProperty("parentConsent", out _), Is.False);
    }

    [Test]
    public async Task GetCurrent_AnEndedLink_StaysPendingAndTheProfileStaysHidden()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveExpiredParentConsent(factory, profileId);
        using HttpClient ownerClient = ClientFor(factory, owner);
        using HttpClient stranger = ClientFor(factory, SaveAccount(factory));

        using HttpResponseMessage currentResponse = await ownerClient.GetAsync(CurrentAddress);
        using JsonDocument currentBody = await ReadJson(currentResponse);
        using HttpResponseMessage strangerResponse = await stranger.GetAsync($"/api/member-profiles/{profileId}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                currentBody.RootElement.GetProperty("data").GetProperty("parentConsent").GetString(),
                Is.EqualTo("pending"));
            Assert.That(strangerResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

    [Test]
    public async Task GetCurrent_PendingParentConsent_NeverHoldsTheParentAddressOrTheToken()
    {
        using WebApplicationFactory<Program> factory = CreateFactory();
        Guid owner = SaveAccount(factory);
        Guid profileId = SaveProfile(factory, owner, AgeBand.Adolescent);
        SaveParentConsent(factory, profileId);
        using HttpClient ownerClient = ClientFor(factory, owner);

        using HttpResponseMessage response = await ownerClient.GetAsync(CurrentAddress);
        string content = await response.Content.ReadAsStringAsync();

        Assert.That(
            content,
            Does.Not.Contain("parent@example.com").And.Not.Contain("expiresAt").And.Not.Contain("givenAt"));
    }

    private static async Task<string> NormalizedProblemAsync(HttpResponseMessage response)
    {
        using JsonDocument body = await ReadJson(response);
        var properties = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (JsonProperty property in body.RootElement.EnumerateObject())
        {
            if (property.Name != "traceId")
            {
                properties[property.Name] = property.Value.ToString();
            }
        }

        return JsonSerializer.Serialize(properties);
    }
}
