using System.Text;
using HouseHelp.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace HouseHelp.Infrastructure.Services;

public class RedisSlotLockService : ISlotLockService
{
    private readonly IDistributedCache _cache;

    public RedisSlotLockService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<bool> AcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        var value = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
        var existing = await _cache.GetAsync(key, cancellationToken);
        if (existing is not null)
        {
            return false;
        }

        await _cache.SetAsync(key, value, options, cancellationToken);
        return true;
    }

    public Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
