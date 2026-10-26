using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Connect.Infrastructure.Tests.Persistence;

internal static class DatabaseMigration
{
    // Another test project can make the test database at the same moment. This connection may
    // then have loaded the types of PostgreSQL before citext existed (#69), so they are loaded
    // again, or every citext parameter fails.
    public static void MigrateAndReloadTypes(this ConnectDbContext dbContext)
    {
        dbContext.Database.Migrate();
        dbContext.Database.OpenConnection();
        ((NpgsqlConnection)dbContext.Database.GetDbConnection()).ReloadTypes();
        dbContext.Database.CloseConnection();
    }
}
