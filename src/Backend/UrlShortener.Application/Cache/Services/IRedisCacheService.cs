namespace UrlShortener.Application.Cache.Services;

public interface IRedisCacheService
{
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T> GetAsync<T>(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<bool> RemoveAsync(string key);
    Task<long> IncrementAsync(string key);
    Task<TimeSpan?> GetTimeToLiveAsync(string key);
}