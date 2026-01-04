using Medallion.Threading;
using Medallion.Threading.Postgres;

namespace DotNetUnknown.Lock;

public static class LockExtension
{
    public static void RegisterLock(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // synchronize lock
        serviceCollection.AddSingleton<SynchronizedLockService>();

        // distributed lock
        serviceCollection
            .AddSingleton<IDistributedLockProvider>(_ =>
            {
                var connectionString = configuration.GetConnectionString("PostgresConnection")!;
                return new PostgresDistributedSynchronizationProvider(connectionString);
            });
        serviceCollection.AddSingleton<DistributedLockService>();
    }
}