namespace UrlShortener.Application.UrlShortening.DTOs.Responses;

public record ShortenUrlResponse(string OriginalUrl, string ShortUrl, string ShortCode, DateTime CreatedAt, int RemainingShortenings, DateTime? ExpiresAt);
