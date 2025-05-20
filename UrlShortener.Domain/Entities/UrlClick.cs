namespace UrlShortener.Domain.Entities;

public class UrlClick
{
    public DateTime ClickedAt { get; set; } = DateTime.Now;
    public string IpAddress { get; set; } = string.Empty;
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? CountryCode { get; set; }
    public string? DeviceType { get; set; }
}