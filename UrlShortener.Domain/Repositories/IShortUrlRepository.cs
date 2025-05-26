using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IShortUrlRepository
{
    Task<ShortUrl> GetByIdAsync(string id);
    Task<ShortUrl> GetByShortCodeAsync(string shortCode);
    Task<IEnumerable<ShortUrl>> GetByUserIdAsync(string userId);
    Task<string> GetShortCodeByOriginalUrlAsync(string originalUrl);
    Task AddASync(ShortUrl shortUrl);
    Task UpdateAsync(ShortUrl shortUrl);
    Task<bool> ShortCodeExistsAsync(string shortCode);
}