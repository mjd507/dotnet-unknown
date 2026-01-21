namespace DotNetUnknown.Caching;

public static class CachingExtension
{
    public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMemoryCache()
            .AddScoped<LocalCacheService>();
        services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "DotNetUnknown:";
            })
            .AddScoped<RedisCacheService>();
    }
}