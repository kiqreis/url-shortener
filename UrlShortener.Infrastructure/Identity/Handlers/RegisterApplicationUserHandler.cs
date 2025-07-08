using Microsoft.AspNetCore.Identity;
using UrlShortener.Application.Users.DTOs.Requests;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class RegisterApplicationUserHandler(UserManager<ApplicationUser> userManager, AuthenticationHandler authenticationHandler)
{
    public async Task<ApplicationUser> CreateUserAsync(CreateUserRequest request)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
        };
        
        var result = await userManager.CreateAsync(applicationUser, request.Password);
        
        if (result.Succeeded)
            return applicationUser;

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        
        throw new InvalidOperationException($"Registration failed for user {request.Email} with errors {errors}");
    }
}