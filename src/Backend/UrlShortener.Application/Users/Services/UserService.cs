using UrlShortener.Application.Users.DTOs.Responses;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Users.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserProfileResponse?> GetByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        return user is null ? throw new InvalidOperationException("User not found") : new UserProfileResponse
        {
            Email = user.Email,
            Plan = user.Plan
        };
    }
}
