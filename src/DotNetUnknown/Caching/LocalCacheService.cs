using DotNetUnknown.Support;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetUnknown.Caching;

public class LocalCacheService(IMemoryCache cache, ILogger<LocalCacheService> logger, ITestSupport testSupport)
{
    public UserData GetUser(int userId)
    {
        var user = cache.GetOrCreate(userId, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SetPriority(CacheItemPriority.Normal);
            return GetUserFromDatabase(userId);
        });
        return user!;
    }

    private UserData GetUserFromDatabase(int userId)
    {
        logger.LogInformation("GetUserFromDatabase {UserId}", userId);
        testSupport.Ack(userId.ToString());
        return new UserData(userId, "JD", 25);
    }

    public record UserData(int Id, string Name, int Age);
}