using StackExchange.Redis;
using UrlShortener.Application.Serialization;
using UrlShortener.Application.Services.Cache;

namespace UrlShortener.Infrastructure.Cache;

public class RedisCacheService(IConnectionMultiplexer redis, IJsonSerializer jsonSerializer) : IRedisCacheService
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serialized = jsonSerializer.Serialize(value);

        return await _database.StringSetAsync(key, serialized, expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        return value.HasValue ? jsonSerializer.Deserialize<T>(value) : default!;
    }

    public async Task<bool> KeyExistsAsync(string key) => await _database.KeyExistsAsync(key);

    public async Task<bool> RemoveAsync(string key) => await _database.KeyDeleteAsync(key);

    public async Task<long> IncrementAsync(string key) => await _database.StringIncrementAsync(key);

    public async Task<TimeSpan?> GetTimeToLiveAsync(string key) => await _database.KeyTimeToLiveAsync(key);
}