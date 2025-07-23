using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Users.DTOs.Responses;

public class LoginResponse
{
    public string Email { get; set; } = string.Empty;
    public UserPlan Plan { get; set; } = UserPlan.Free;
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
}