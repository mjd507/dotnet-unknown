using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.DbConfig;

public static class AppDbContextExtension
{
    extension(IServiceCollection serviceCollection)
    {
        public void RegisterAppDbContext()
        {
            serviceCollection.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=MyDb.db"));
        }
    }

    extension(WebApplication webApplication)
    {
        public void EnsureDatabaseCreated()
        {
            using var scope = webApplication.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }
    }
}