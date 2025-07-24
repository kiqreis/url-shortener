namespace UrlShortener.Application.Common.Security;

public class JwtTokenResult
{
    public string Value { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = [];
}