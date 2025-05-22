namespace UrlShortener.Domain.Entities;

public class UrlClick : EntityBase
{
    public DateTime ClickedAt { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string? Referrer { get; private set; }
    public string? UserAgent { get; private set; }
    public string? CountryCode { get; private set; }
    public string? DeviceType { get; private set; }

    public ShortUrl? ShortUrl { get; private set; }
    public Guid ShortUrlId { get; private set; }

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