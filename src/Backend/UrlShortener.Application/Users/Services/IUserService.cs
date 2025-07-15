using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Users.Services;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email); 
}
