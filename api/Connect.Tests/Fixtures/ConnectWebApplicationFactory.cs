using Connect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Connect.Tests.Fixtures;

public sealed class ConnectWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        using IServiceScope scope = host.Services.CreateScope();
        ConnectDbContext dbContext = scope.ServiceProvider.GetRequiredService<ConnectDbContext>();
        dbContext.Database.Migrate();

        return host;
    }
}
