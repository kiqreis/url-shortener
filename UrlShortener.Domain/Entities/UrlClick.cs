namespace UrlShortener.Domain.Entities;

public class UrlClick : EntityBase
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

    public void SetGeoData(string countryCode) => CountryCode = countryCode;

    public void SetDeviceData(string deviceType) => DeviceType = deviceType;
}