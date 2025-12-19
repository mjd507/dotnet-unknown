using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.DbConfig;

public static class AppDbContextExtension
{
    extension(IServiceCollection serviceCollection)
    {
        public void RegisterAppDbContext(IConfiguration configuration)
        {
            serviceCollection.AddDbContext<AppDbContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("PostgresConnection"))
            );
        }
    }

    extension(WebApplication webApplication)
    {
        public void EnsureDatabaseCreated()
        {
            // Since now uses Docker Compose with Postgres, tables are created before app runs.
            // below for SqlLite are not required.
            // using var scope = webApplication.Services.CreateScope();
            // var services = scope.ServiceProvider;
            // var context = services.GetRequiredService<AppDbContext>();
            // context.Database.EnsureCreated();
        }
    }
}