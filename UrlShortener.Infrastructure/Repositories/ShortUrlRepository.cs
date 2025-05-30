using StackExchange.Redis;
using System.Text.Json;
using UrlShortener.Application.Serialization;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure.Repositories;

public class ShortUrlRepository(IConnectionMultiplexer redis, IJsonSerializer jsonSerializer) : IShortUrlRepository
{
    private readonly IDatabase _redis = redis.GetDatabase();
    private const string UrlKeyPrefix = "url:";
    private const string CodeKeyPrefix = "code:";

    public async Task<ShortUrl> GetByIdAsync(string id)
    {
        var json = await _redis.StringGetAsync($"{UrlKeyPrefix}{id}");

        return json.HasValue ? jsonSerializer.Deserialize<ShortUrl>(json) : null!;
    }

    public async Task<ShortUrl> GetByShortCodeAsync(string shortCode)
    {
        var urlId = await _redis.StringGetAsync($"{CodeKeyPrefix}{shortCode}");

        if (!urlId.HasValue) return null!;

        return await GetByIdAsync(urlId);
    }

    public async Task<bool> AddAsync(ShortUrl shortUrl)
    {
        var transaction = _redis.CreateTransaction();

        var urlKey = $"{UrlKeyPrefix}{shortUrl.Id}";
        var codeKey = $"{CodeKeyPrefix}{shortUrl.ShortCode}";

        var serialized = jsonSerializer.Serialize(shortUrl);

        var expiry = shortUrl.ExpiresAt.HasValue
            ? TimeSpan.FromSeconds(Math.Max(0, (shortUrl.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds))
            : (TimeSpan?)null;

        var setUrlTask = transaction.StringSetAsync(urlKey, serialized, expiry);
        var setCodeTask = transaction.StringSetAsync(codeKey, shortUrl.Id.ToString(), expiry);

        var committed = await transaction.ExecuteAsync();

        if (committed)
        {
            await Task.WhenAll(setUrlTask, setCodeTask);
        }

        return await transaction.ExecuteAsync();
    }

    public async Task<bool> UpdateAsync(ShortUrl shortUrl)
    {
        var remainingTtl = await _redis.KeyTimeToLiveAsync($"{UrlKeyPrefix}{shortUrl.Id}");

        return await _redis.StringSetAsync(
            $"{UrlKeyPrefix}{shortUrl.Id}",
            jsonSerializer.Serialize(shortUrl),
            remainingTtl);
    }

    public async Task<bool> ShortCodeExistsAsync(string shortCode) =>
        await _redis.KeyExistsAsync($"{CodeKeyPrefix}{shortCode}");

    public async Task<bool> TryAddAsync(ShortUrl url)
    {
        return await _redis.StringSetAsync(
            key: $"url:{url.ShortCode}",
            value: JsonSerializer.Serialize(url),
            when: When.NotExists,
            expiry: url.ExpiresAt.HasValue ? url.ExpiresAt.Value - DateTime.UtcNow : null
        );
    }
}