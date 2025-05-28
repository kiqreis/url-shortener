using UrlShortener.Application.Cache.Services;
using UrlShortener.Application.Common;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.UrlShortening.Services;

public class UrlShorteningService(IShortUrlRepository repository, IBase58Encoder encoder, IRedisCacheService cacheService) : IUrlShorteningService
{
    private const int MaxGenerationAttempts = 5;

    public async Task<ShortUrl> GetAndTrackAsync(TrackUrlRequest request)
    {
        var shortUrl = await GetFromCacheOrRepository(request.ShortCode);

        TrackClick(shortUrl, request.IpAddress, request.Referrer, request.UserAgent);

        await repository.UpdateAsync(shortUrl);
        await UpdateCache(shortUrl);

        return shortUrl;
    }

    public async Task<ShortUrl> ShortenUrlAsync(ShortenUrlRequest request)
    {
        ValidateUrl(request.OriginalUrl);

        var shortCode = request.CustomCode ?? await GenerateUniqueShortCodeAsync();
        var shortUrl = ShortUrl.Create(request.OriginalUrl, shortCode, request.UserId, request.Duration);

        await repository.AddAsync(shortUrl);
        await cacheService.SetAsync($"url:{shortCode}", shortUrl, request.Duration);

        return shortUrl;
    }

    public async Task<string> GenerateUniqueShortCodeAsync()
    {
        string code;
        var attempts = 0;

        do
        {
            attempts++;
            var number = await GetUniqueNumber();
            code = encoder.Encode(number);

            if (attempts >= MaxGenerationAttempts)
            {
                code = GenerateFallbackCode();
                break;
            }

        } while (await repository.ShortCodeExistsAsync(code));

        return code;
    }

    private static void ValidateUrl(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new HttpRequestException("Invalid url format");
    }

    private static void TrackClick(ShortUrl shortUrl, string ipAddress, string? referrer, string? userAgent)
    {
        shortUrl.RegisterClick(ipAddress, referrer, userAgent);
    }

    private static string GenerateFallbackCode() => Guid.NewGuid().ToString("N")[..8];

    private async Task<ShortUrl> GetFromCacheOrRepository(string shortCode)
    {
        var cachedUrl = await cacheService.GetAsync<ShortUrl>($"url:{shortCode}");

        if (cachedUrl != null) return cachedUrl;

        var repositoryUrl = await repository.GetByShortCodeAsync(shortCode);

        return repositoryUrl ?? throw new HttpRequestException("Short url not found");
    }

    private async Task UpdateCache(ShortUrl shortUrl)
    {
        var remainingTTL = await cacheService.GetTimeToLiveAsync($"url:{shortUrl.ShortCode}");

        await cacheService.SetAsync($"url:{shortUrl.ShortCode}", shortUrl, remainingTTL);
    }

    private async Task<long> GetUniqueNumber()
    {
        try
        {
            return await cacheService.IncrementAsync("url-id-counter");
        }
        catch
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }
}
