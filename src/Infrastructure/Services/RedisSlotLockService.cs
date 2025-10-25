using System.Collections.Concurrent;
using HouseHelp.Domain.Services;
using StackExchange.Redis;

namespace HouseHelp.Infrastructure.Services;

public class RedisSlotLockService : ISlotLockService
{
    private static readonly LuaScript ReleaseScript = LuaScript.Prepare(
        "if redis.call('get', @key) == @value then return redis.call('del', @key) else return 0 end");

    private readonly IDatabase _database;
    private readonly ConcurrentDictionary<string, string> _lockTokens = new();

    public RedisSlotLockService(IConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase();
    }

    public async Task<bool> AcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var token = Guid.NewGuid().ToString();
        var acquired = await _database.StringSetAsync(key, token, ttl, When.NotExists);
        if (!acquired)
        {
            return false;
        }

        _lockTokens[key] = token;
        return true;
    }

    public async Task ReleaseAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_lockTokens.TryRemove(key, out var token))
        {
            await _database.ScriptEvaluateAsync(ReleaseScript, new { key, value = token });
        }
        else
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
