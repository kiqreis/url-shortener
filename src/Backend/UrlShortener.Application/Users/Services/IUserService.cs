using UrlShortener.Application.Users.DTOs.Responses;

namespace UrlShortener.Application.Users.Services;

public interface IUserService
{
    Task<UserProfileResponse?> GetByEmailAsync(string email); 
}
