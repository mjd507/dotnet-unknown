namespace DotNetUnknown.Lock;

public static class LockExtension
{
    public static void RegisterLock(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SynchronizedLockService>();
    }
}