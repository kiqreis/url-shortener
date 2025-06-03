using StackExchange.Redis;
using UrlShortener.Application.Serialization;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure.Repositories;

public class ShortUrlRepository(IConnectionMultiplexer redis, IJsonSerializer jsonSerializer) : IShortUrlRepository
{
    private readonly IDatabase _redis = redis.GetDatabase();
    private const string UrlKeyPrefix = "url:";

    public async Task<ShortUrl> GetByShortCodeAsync(string shortCode)
    {
        var json = await _redis.StringGetAsync($"{UrlKeyPrefix}{shortCode}");

        return json.HasValue ? jsonSerializer.Deserialize<ShortUrl>(json) : null!;
    }

    public async Task<bool> AddAsync(ShortUrl shortUrl)
    {
        var urlKey = $"{UrlKeyPrefix}{shortUrl.ShortCode}";
        var serialized = jsonSerializer.Serialize(shortUrl);

        var expiry = shortUrl.ExpiresAt.HasValue
            ? TimeSpan.FromSeconds(Math.Max(0, (shortUrl.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds))
            : (TimeSpan?)null;

        return await _redis.StringSetAsync(urlKey, serialized, expiry, When.NotExists);
    }

    public async Task<bool> UpdateAsync(ShortUrl shortUrl)
    {
        var urlKey = $"{UrlKeyPrefix}{shortUrl.ShortCode}";
        var remainingTtl = await _redis.KeyTimeToLiveAsync(urlKey);

        if (remainingTtl == TimeSpan.Zero || remainingTtl == null)
            remainingTtl = shortUrl.ExpiresAt.HasValue
                ? TimeSpan.FromSeconds(Math.Max(0, (shortUrl.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds))
                : TimeSpan.FromDays(7);

        return await _redis.StringSetAsync(urlKey, jsonSerializer.Serialize(shortUrl), remainingTtl);
    }

    public async Task<bool> ShortCodeExistsAsync(string shortCode) =>
        await _redis.KeyExistsAsync($"{UrlKeyPrefix}{shortCode}");
}