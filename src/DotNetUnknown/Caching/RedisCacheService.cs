using DotNetUnknown.Json;
using DotNetUnknown.Support;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetUnknown.Caching;

public class RedisCacheService(
    IDistributedCache distributedCache,
    ILogger<RedisCacheService> logger,
    ITestSupport testSupport)
{
    public UserData GetUser(int userId)
    {
        var userData = distributedCache.GetString(userId.ToString());
        if (userData != null) return JsonUtils.Deserialize<UserData>(userData)!;
        var userFromDatabase = GetUserFromDatabase(userId);
        distributedCache.SetString(userId.ToString(), JsonUtils.Serialize(userFromDatabase),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
            });
        return userFromDatabase;
    }

    private UserData GetUserFromDatabase(int userId)
    {
        logger.LogInformation("GetUserFromDatabase {UserId}", userId);
        testSupport.Ack(userId.ToString());
        return new UserData(userId, "JD", 25);
    }

    public record UserData(int Id, string Name, int Age);
}