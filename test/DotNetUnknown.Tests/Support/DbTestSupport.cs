using DotNetUnknown.DbConfig;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetUnknown.Tests.Support;

public abstract class DbTestSupport : BaseTestSupport
{
    public required SqliteConnection SqliteConnection;

    protected override Action<IServiceCollection> ConfigureTestServicesAction()
    {
        return services =>
        {
            // Remove Application's DbContext
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            // Register a new DbContext for integration testing
            SqliteConnection = new SqliteConnection("DataSource=:memory:");
            SqliteConnection.Open(); // need to keep conn open for in-memory db
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(SqliteConnection));
        };
    }

    [OneTimeSetUp]
    public void DbSetup()
    {
    }

    [OneTimeTearDown]
    public void DbTeardown()
    {
        SqliteConnection.Close();
        SqliteConnection.Dispose();
    }
}