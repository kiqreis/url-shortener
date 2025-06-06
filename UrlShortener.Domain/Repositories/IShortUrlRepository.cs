using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IShortUrlRepository
{
    Task<ShortUrl> GetByShortCodeAsync(string shortCode);
    Task<bool> AddAsync(ShortUrl shortUrl);
    Task<bool> UpdateAsync(ShortUrl shortUrl);
    Task<bool> ShortCodeExistsAsync(string shortCode);
}