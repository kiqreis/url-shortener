using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities;

public class ShortUrl : EntityBase
{
    public string OriginalUrl { get; private set; } = string.Empty;
    public string ShortCode { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; private set; }
    public int ClickCount { get; private set; }
    public bool IsActive { get; private set; }

    public UrlStatus UrlStatus { get; private set; } = UrlStatus.Active;
    public IReadOnlyCollection<UrlClick> Clicks => _clicks.AsReadOnly();

    public User User { get; private set; } = null!;
    public Guid UserId { get; private set; }

    private readonly List<UrlClick> _clicks = [];

    private ShortUrl(string originalUrl, string shortCode, Guid userId, DateTime? expiresAt = null)
    {
        OriginalUrl = originalUrl ?? throw new ArgumentNullException(nameof(originalUrl));
        ShortCode = shortCode ?? throw new ArgumentNullException(nameof(shortCode));
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
        UrlStatus = UrlStatus.Active;
    }

    public static ShortUrl Create(string originalUrl, string shortCode, Guid userId, TimeSpan? duration = null)
    {
        DateTime? expiresAt = duration.HasValue ? DateTime.UtcNow.Add(duration.Value) : null;
        return new ShortUrl(originalUrl, shortCode, userId, expiresAt);
    }

    public void RegisterClick(string ipAddress, string? referrer = null, string? userAgent = null)
    {
        _clicks.Add(UrlClick.Create(this, ipAddress, referrer, userAgent));
        ClickCount++;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        UrlStatus = UrlStatus.Inactive;
    }

    public void Expire()
    {
        ExpiresAt = DateTime.UtcNow;
        UrlStatus = UrlStatus.Expired;
    }

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt < DateTime.UtcNow;
}