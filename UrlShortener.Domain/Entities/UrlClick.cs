namespace UrlShortener.Domain.Entities;

public class UrlClick
{
    public DateTime ClickedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? CountryCode { get; set; }
    public string? DeviceType { get; set; }

    public ShortUrl? ShortUrl { get; set; }
    public Guid ShortUrlId { get; set; }

    public static UrlClick Create(ShortUrl shortUrl, string ipAddress, string? referrer, string? userAgent)
    {
        return new UrlClick
        {
            ShortUrlId = shortUrl.Id,
            ClickedAt = DateTime.Now,
            IpAddress = ipAddress,
            Referrer = referrer,
            UserAgent = userAgent
        };
    }   
}