using System.Net;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Connect.Infrastructure.Tests.Persistence;
using Connect.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Connect.Tests.Profiles.Endpoints;

// Runs against a real test database and records every command EF Core sends, so it proves a
// visitor who opens a member address never causes a read of the table profile (#347). Until log
// in (#15) a visitor is a request without the header X-Temporary-Account-Id.
public sealed class VisitorDatabaseTests
{
    private const string UnknownId = "00000000-0000-4000-8000-000000000999";
    private const string NotAnId = "abc";

    [Test]
    public async Task Visitor_OpeningKnownUnknownOrMalformedId_NeverCausesACommandOnProfile()
    {
        var recorder = new DatabaseCommandRecorder();
        using WebApplicationFactory<Program> factory = FactoryWithRecorder(recorder);
        Guid accountId = ProfileTestHelpers.SaveAccount(factory);
        Guid profileId = ProfileTestHelpers.SaveProfile(factory, accountId, AgeBand.YoungAdult);
        using HttpClient client = factory.CreateClient();

        await client.GetAsync($"/api/member-profiles/{profileId}");
        await client.GetAsync($"/api/member-profiles/{UnknownId}");
        await client.GetAsync($"/api/member-profiles/{NotAnId}");

        Assert.That(recorder.CommandsFor("profile"), Is.Empty);
    }

    [Test]
    public async Task LoggedInMember_OpeningAnExistingProfile_CausesACommandOnProfile()
    {
        var recorder = new DatabaseCommandRecorder();
        using WebApplicationFactory<Program> factory = FactoryWithRecorder(recorder);
        Guid ownerAccountId = ProfileTestHelpers.SaveAccount(factory);
        Guid profileId = ProfileTestHelpers.SaveProfile(factory, ownerAccountId, AgeBand.YoungAdult);
        using HttpClient client = ProfileTestHelpers.ClientFor(factory, ProfileTestHelpers.SaveAccount(factory));

        using HttpResponseMessage response = await client.GetAsync($"/api/member-profiles/{profileId}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(recorder.CommandsFor("profile"), Is.Not.Empty);
        }
    }

    // Attaches the recorder to the same ConnectDbContext the running server uses, by registering
    // it again on the test database of TestsConnection (the last registration wins).
    private static WebApplicationFactory<Program> FactoryWithRecorder(DatabaseCommandRecorder recorder)
    {
        string connectionString = TestDatabase.ConnectionString();
        return new ConnectWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);
            builder.UseSetting("TemporaryAccount:Enabled", "true");
            builder.ConfigureServices(services => services.AddDbContext<ConnectDbContext>(options => options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(recorder)));
        });
    }
}
