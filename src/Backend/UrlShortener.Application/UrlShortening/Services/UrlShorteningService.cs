using System.Security.Cryptography;
using UrlShortener.Application.Cache.Services;
using UrlShortener.Application.Common;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.DTOs.Responses;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.UrlShortening.Services;

public class UrlShorteningService(
    IShortUrlRepository repository,
    IBase58Encoder encoder,
    IRedisCacheService cacheService,
    IUserRepository userRepository) : IUrlShorteningService
{
    private const int MaxGenerationAttempts = 5;
    private const int MinShortCodeLength = 7;
    private const int RandomBytesLength = 5;
    private const int MaxUrlsPerIp = 10;
    private const string IpLimitKeyPrefix = "anon-ip-limit";

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromDays(7);

    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? "http://localhost:5181"
            : "https://cute.link";

    public async Task<ShortenUrlResponse> GetAndTrackAsync(TrackUrlRequest request)
    {
        var shortCode = ExtractShortCode(request.ShortCode);
        var shortUrl = await GetFromCacheOrRepository(shortCode);

        shortUrl.RegisterClick(request.IpAddress, request.Referrer, request.UserAgent);

        await repository.UpdateAsync(shortUrl);
        await UpdateCache(shortUrl);

        return await CreateResponse(shortUrl);
    }

    public async Task<ShortenUrlResponse> ShortenUrlAsync(ShortenUrlRequest request)
    {
        ValidateUrl(request.OriginalUrl);

        await ValidateAndApplyLimitAsync(request.UserEmail, request.IpAddress);

        var shortCode = request.CustomCode != null
            ? await HandleCustomCode(request.CustomCode)
            : await GenerateUniqueShortCodeAsync();

        var duration = request.Duration ?? DefaultDuration;

        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be greater than zero");

        var userId = Guid.Empty;

        if (!string.IsNullOrWhiteSpace(request.UserEmail))
        {
            var user = await userRepository.GetByEmailAsync(request.UserEmail);

            if (user is not null)
                userId = user.Id;
        }

        var shortUrl = ShortUrl.Create(
            request.OriginalUrl,
            shortCode,
            userId,
            duration);

        await InsertShortUrlWithRetry(shortUrl);
        await cacheService.SetAsync($"url:{shortCode}", shortUrl, duration);

        return await CreateResponse(shortUrl, request.UserEmail, request.IpAddress);
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
            var bytes = new byte[RandomBytesLength];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(bytes);
            }

            var randomNumber = BitConverter.ToInt64([.. new byte[8].Select((b, i) =>
                i < bytes.Length ? bytes[i] : (byte)0)], 0) & long.MaxValue;

            code = encoder.Encode(randomNumber);

            while (code.Length < MinShortCodeLength)
            {
                var additionalBytes = new byte[1];

                using (var randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    randomNumberGenerator.GetBytes(additionalBytes);
                }

                code += encoder.Encode(additionalBytes[0] % Base58Encoder.Base);
            }

            if (code.Length > MinShortCodeLength + 2)
                code = code[..(MinShortCodeLength + 2)];

            if (++attempts < MaxGenerationAttempts) continue;
            
            code = GenerateFallbackCode();
            break;
        } while (await repository.ShortCodeExistsAsync(code));

        return code;
    }

    private async Task InsertShortUrlWithRetry(ShortUrl shortUrl)
    {
        const int maxRetries = 3;

        for (var i = 0; i < maxRetries; i++)
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

    private async Task<int> GetRemainingLimitAsync(string email, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return MaxUrlsPerIp;

            var ipKey = $"{IpLimitKeyPrefix}:{ipAddress}:{DateTime.UtcNow:yyyyMMdd}";
            var currentCount = await cacheService.GetAsync<int>(ipKey);

            var remaining = MaxUrlsPerIp - currentCount;
            return remaining < 0 ? 0 : remaining;
        }
        else
        {
            var user = await userRepository.GetByEmailAsync(email);
            if (user == null)
                return 0;

            var userLimit = GetUserDailyLimit(user.Plan);
            var userCacheKey = $"user_limits:{email}:{DateTime.UtcNow:yyyyMMdd}";

            var currentCount = await cacheService.GetAsync<int>(userCacheKey);

            var remaining = userLimit - currentCount;
            return remaining < 0 ? 0 : remaining;
        }
    }

    private static int GetUserDailyLimit(UserPlan userPlan)
    {
        return userPlan switch
        {
            UserPlan.Free => 30,
            UserPlan.Pro => 1_000,
            UserPlan.Business => int.MaxValue,
            _ => 30
        };
    }

    private async Task ValidateAndApplyLimitAsync(string? email, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP Address is required for anonymous users to apply rate limit.");

            var ipKey = $"{IpLimitKeyPrefix}:{ipAddress}:{DateTime.UtcNow:yyyyMMdd}";
            var count = await cacheService.GetAsync<int>(ipKey);

            if (count >= MaxUrlsPerIp)
                throw new Exception("Daily limit of briefing achieved for this io");

            var newCount = await cacheService.IncrementAsync(ipKey);

            if (newCount == 1)
                await cacheService.SetAsync(ipKey, newCount, TimeSpan.FromDays(1));
        }
        else
        {
            var user = await userRepository.GetByEmailAsync(email) ?? throw new ArgumentException("User not found");
            var userLimit = GetUserDailyLimit(user.Plan);
            var userCacheKey = $"user_limits:{email}:{DateTime.UtcNow:yyyyMMdd}";
            var currentCount = await cacheService.GetAsync<int>(userCacheKey);

            if (currentCount >= userLimit)
                throw new Exception("Daily limit of briefing achieved for this user");

            var newCount = await cacheService.IncrementAsync(userCacheKey);

            if (newCount == 1)
                await cacheService.SetAsync(userCacheKey, newCount, TimeSpan.FromDays(1));
        }
    }

    private async Task<ShortenUrlResponse> CreateResponse(ShortUrl shortUrl, string? email = null, string? ipAddress = null)
    {
        var remainingShortenings = await GetRemainingLimitAsync(email!, ipAddress);

        return new(
            OriginalUrl: shortUrl.OriginalUrl,
            ShortCode: shortUrl.ShortCode,
            ShortUrl: $"{BaseUrl}/v1/urls/{shortUrl.ShortCode}",
            CreatedAt: shortUrl.CreatedAt,
            RemainingShortenings: remainingShortenings,
            ExpiresAt: shortUrl.ExpiresAt
        );
    }
}