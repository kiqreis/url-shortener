using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.DTOs.Responses;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.UrlShortening.Services;

public interface IUrlShorteningService
{
    Task<ShortenUrlResponse> ShortenUrlAsync(ShortenUrlRequest request);
    Task<ShortUrl> GetAndTrackAsync(TrackUrlRequest request);
    Task<string> GenerateUniqueShortCodeAsync();
}
