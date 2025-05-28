namespace UrlShortener.Application.UrlShortening.DTOs.Requests;

public record TrackUrlRequest(string ShortCode, string IpAddress, string? Referrer = null, string? UserAgent = null);
