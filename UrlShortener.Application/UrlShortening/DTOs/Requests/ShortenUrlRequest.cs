using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UrlShortening.DTOs.Requests;

public record ShortenUrlRequest(string OriginalUrl, Guid UserId, string? CustomCode = null, TimeSpan? Duration = null)
{
    public static implicit operator ShortUrl(ShortenUrlRequest request) =>
        ShortUrl.Create(request.OriginalUrl, request.CustomCode ?? string.Empty, request.UserId, request.Duration);
}
