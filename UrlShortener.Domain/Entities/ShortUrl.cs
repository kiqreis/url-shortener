using UrlShortener.Domain.Enums;

namespace UrlShortener.Domain.Entities;

public class ShortUrl : EntityBase
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ExpiresAt { get; set; }
    public int ClickCount { get; set; }
    public bool IsActive { get; set; }

    public UrlStatus UrlStatus { get; set; } = UrlStatus.Active;
    public IReadOnlyCollection<UrlClick> Clicks => _clicks.AsReadOnly();

    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

    private readonly List<UrlClick> _clicks = [];

    public void RegisterClick(string ipAddress, string? referrer = null, string? userAgent = null)
    {
        _clicks.Add(UrlClick.Create(this, ipAddress, referrer, userAgent));
        ClickCount++;
    }
}