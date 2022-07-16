using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SharedModule.Extensions;

public static class CahceExtensions
{
    public static async Task SaveToCacheAsync<TKey, TValue>(this IDistributedCache cache, TKey key, TValue value, string moduleName, CancellationToken cancellation)
    {
        await cache.SetStringAsync($"{moduleName}-{key}", JsonSerializer.Serialize(value), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        }, cancellation);
    }

    public static async Task<TValue?> GetFromCacheAsync<TKey, TValue>(this IDistributedCache cache, TKey key, string moduleName, CancellationToken cancellation)
    {
        string? cachedValue = await cache.GetStringAsync($"{moduleName}-{key}", cancellation);

        if (cachedValue != null)
        {
            return JsonSerializer.Deserialize<TValue>(cachedValue);
        }
        return default;
    }
}
