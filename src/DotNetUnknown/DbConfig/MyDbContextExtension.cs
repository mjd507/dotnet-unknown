using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.DbConfig;

public static class MyDbContextExtension
{
    extension(IServiceCollection serviceCollection)
    {
        public void RegisterMyDbContext()
        {
            serviceCollection.AddDbContext<MyDbContext>(options => options.UseSqlite("Data Source=MyDb.db"));
        }
    }

    extension(WebApplication webApplication)
    {
        public void EnsureDatabaseCreated()
        {
            using var scope = webApplication.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<MyDbContext>();
            context.Database.EnsureCreated();
        }
    }
}