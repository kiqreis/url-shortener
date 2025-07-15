using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Application.Users.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User?> GetByEmailAsync(string email) => await userRepository.GetByEmailAsync(email);
}
