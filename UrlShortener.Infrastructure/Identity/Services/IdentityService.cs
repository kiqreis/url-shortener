using Microsoft.AspNetCore.Identity;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.DTOs.Responses;
using UrlShortener.Application.Users.Services;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure.Identity.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserRepository userRepository)
    : IIdentityService
{
    public async Task<CreateUserResponse> RegisterAsync(CreateUserRequest request)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(applicationUser, request.Password);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Registration failed for user {request.Email} with errors {errorMessages}");
        }

        var user = User.Create(applicationUser.Id, request.Email);

        await userRepository.AddAsync(user);
        await signInManager.SignInAsync(applicationUser, isPersistent: true);

        return new CreateUserResponse
        {
            Email = user.Email,
            Plan = user.Plan
        };
    }
}