namespace UrlShortener.Domain.Common.Security;

public class JwtToken
{
    public string Value { get; set; } = string.Empty;
    public DateTime ValidForm { get; set; }
    public DateTime ValidTo { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
}