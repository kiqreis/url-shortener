using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class RegisterUserHandler(IUserRepository userRepository)
{
    public async Task<User> CreateUserAsync(ApplicationUser applicationUser, CreateUserRequest request)
    {
        var user = User.Create(applicationUser.Id, request.Email);
        
        await userRepository.AddAsync(user);

        return user;
    }
}