namespace UrlShortener.Application.Users.DTOs.Requests;

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public int? NewExpiryInMinutes { get; set; }
}