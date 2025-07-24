namespace UrlShortener.Application.Users.DTOs.Responses;

public class RefreshTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public string Subject { get; set; } = string.Empty;
}
