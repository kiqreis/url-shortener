using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Users.DTOs.Responses;

public class CreateUserResponse
{
   public string Email { get; set; } = null!;
   public UserPlan Plan { get; set; }
   public string Token { get; set; } = string.Empty;
}