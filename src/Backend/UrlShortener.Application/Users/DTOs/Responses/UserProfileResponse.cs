using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Application.Users.DTOs.Responses;

public class UserProfileResponse
{
    public string Email { get; set; } = string.Empty;
    public UserPlan Plan { get; set; } = UserPlan.Free;
}
