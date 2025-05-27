using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Services.UrlShortening;

public interface IUrlShorteningService
{
    Task<ShortUrl> ShortenUrlAsync(string originalUrl, Guid userId, string? customCode = null, TimeSpan? duration = null);
    Task<ShortUrl> GetAndTrackAsync(string shortCode, string ipAddress, string? referrer = null, string? userAgent = null);
    Task<string> GenerateUniqueShortCodeAsync();
}
