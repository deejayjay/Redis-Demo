using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RedisDemo.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache, 
        string recordId,
        T data,
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? inactivityExpireTime = null)
    {
        var jsonData = JsonSerializer.Serialize(data);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(180),
            SlidingExpiration = inactivityExpireTime ?? TimeSpan.FromSeconds(120)
        };
        
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);

        if (jsonData is null)
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(jsonData)!;
    }
}
