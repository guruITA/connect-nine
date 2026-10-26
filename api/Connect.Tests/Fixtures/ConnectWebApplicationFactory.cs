using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Connect.Tests.Fixtures;

public sealed class ConnectWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        using IServiceScope scope = host.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        dbContext.Database.Migrate();

        // Another test project can make the test database at the same moment. This host may then
        // have loaded the types of PostgreSQL before citext existed (#69), so they are loaded
        // again, or every citext parameter fails.
        dbContext.Database.OpenConnection();
        ((NpgsqlConnection)dbContext.Database.GetDbConnection()).ReloadTypes();
        dbContext.Database.CloseConnection();

        return host;
    }
}
