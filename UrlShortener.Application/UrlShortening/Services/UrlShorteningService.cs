using System.Security.Cryptography;
using UrlShortener.Application.Cache.Services;
using UrlShortener.Application.Common;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.DTOs.Responses;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.UrlShortening.Services;

public class UrlShorteningService(
    IShortUrlRepository repository,
    IBase58Encoder encoder,
    IRedisCacheService cacheService) : IUrlShorteningService
{
    private const int MaxGenerationAttempts = 5;
    private const int MinShortCodeLength = 4;
    private const string UrlIdCounterKey = "url-id-counter";
    private const int MaxUrlsPerIp = 10;
    private const string IpLimitKeyPrefix = "anon-ip-limit";

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromDays(7);

    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? "http://localhost:5181/"
            : "https://cut.link";

    public async Task<ShortenUrlResponse> GetAndTrackAsync(TrackUrlRequest request)
    {
        var shortCode = ExtractShortCode(request.ShortCode);
        var shortUrl = await GetFromCacheOrRepository(shortCode);

        shortUrl.RegisterClick(request.IpAddress, request.Referrer, request.UserAgent);

        await repository.UpdateAsync(shortUrl);
        await UpdateCache(shortUrl);

        return CreateResponse(shortUrl);
    }

    public async Task<ShortenUrlResponse> ShortenUrlAsync(ShortenUrlRequest request)
    {
        ValidateUrl(request.OriginalUrl);

        var isAnonymous = request.UserId == null || request.UserId == Guid.Empty;

        if (isAnonymous)
        {
            if (string.IsNullOrWhiteSpace(request.IpAddress))
            {
                throw new ArgumentException("IP Address is required for anonymous users to apply rate limit.");
            }

            var ipKey = $"{IpLimitKeyPrefix}:{request.IpAddress}:{DateTime.UtcNow:yyyyMMdd}";
            var count = await cacheService.GetAsync<int>(ipKey);

            if (count >= MaxUrlsPerIp)
                throw new Exception("Daily limit of briefting achieved for this ip");

            var newCount = await cacheService.IncrementAsync(ipKey);

            if (newCount == 1)
            {
                await cacheService.SetAsync(ipKey, newCount, TimeSpan.FromDays(1));
            }
        }

        var shortCode = request.CustomCode != null
            ? await HandleCustomCode(request.CustomCode)
            : await GenerateUniqueShortCodeAsync();

        var duration = request.Duration ?? DefaultDuration;

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero");

        var shortUrl = ShortUrl.Create(
            request.OriginalUrl,
            shortCode,
            request.UserId ?? Guid.Empty,
            duration);

        await InsertShortUrlWithRetry(shortUrl);
        await cacheService.SetAsync($"url:{shortCode}", shortUrl, duration);

        return CreateResponse(shortUrl);
    }

    private async Task<string> HandleCustomCode(string customCode)
    {
        if (!IsValidCustomCode(customCode))
            throw new ArgumentException("Custom code contains invalid characters");

        if (await repository.ShortCodeExistsAsync(customCode))
            throw new ArgumentException("Custom code already exists");

        return customCode;
    }

    private static bool IsValidCustomCode(string code) =>
        code.All(c => char.IsLetterOrDigit(c) || c is '-' or '_');

    public async Task<string> GenerateUniqueShortCodeAsync()
    {
        var attempts = 0;
        var random = new Random();
        string code;

        do
        {
            var number = await GetUniqueNumber();
            code = encoder.Encode(number);

            while (code.Length < MinShortCodeLength)
            {
                code += encoder.Encode(random.Next(1, 58));
            }

            if (++attempts < MaxGenerationAttempts) continue;
            code = GenerateFallbackCode();
            break;
        } while (await repository.ShortCodeExistsAsync(code));

        return code;
    }

    private async Task InsertShortUrlWithRetry(ShortUrl shortUrl)
    {
        const int maxRetries = 3;

        for (int i = 0; i < maxRetries; i++)
        {
            if (await repository.AddAsync(shortUrl))
                return;

            if (i < maxRetries - 1)
            {
                shortUrl.UpdateShortCode(await GenerateUniqueShortCodeAsync());
            }
        }

        throw new Exception("Failed to create short URL after retries");
    }

    private static void ValidateUrl(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new HttpRequestException("Invalid URL format");
    }

    private static string GenerateFallbackCode() =>
        Guid.NewGuid().ToString("N")[..8];

    private static string ExtractShortCode(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out var uri) ? uri.Segments.Last().Trim('/') : input.Trim('/');
    }


    private async Task<ShortUrl> GetFromCacheOrRepository(string shortCode) =>
        await cacheService.GetAsync<ShortUrl>($"url:{shortCode}");

    private async Task UpdateCache(ShortUrl shortUrl)
    {
        var remainingTTL = await cacheService.GetTimeToLiveAsync($"url:{shortUrl.ShortCode}");

        if (remainingTTL == TimeSpan.Zero)
            remainingTTL = DefaultDuration;

        await cacheService.SetAsync($"url:{shortUrl.ShortCode}", shortUrl, remainingTTL);
    }

    private async Task<long> GetUniqueNumber()
    {
        try
        {
            if (!await cacheService.KeyExistsAsync(UrlIdCounterKey))
                await cacheService.SetAsync(UrlIdCounterKey, 1_000_000L);

            return await cacheService.IncrementAsync(UrlIdCounterKey);
        }
        catch
        {
            var bytes = new byte[8];
            RandomNumberGenerator.Fill(bytes);

            return BitConverter.ToInt64(bytes, 0) & long.MaxValue;
        }
    }

    private static ShortenUrlResponse CreateResponse(ShortUrl shortUrl) =>
        new(
            OriginalUrl: shortUrl.OriginalUrl,
            ShortCode: shortUrl.ShortCode,
            ShortUrl: $"{BaseUrl}/v1/short-urls/{shortUrl.ShortCode}",
            CreatedAt: shortUrl.CreatedAt,
            ExpiresAt: shortUrl.ExpiresAt
        );
}