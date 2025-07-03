using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.DTOs.Responses;
using UrlShortener.Application.Users.Services;
using UrlShortener.Domain.Common.Security;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Identity.Constants;

namespace UrlShortener.Infrastructure.Identity.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserRepository userRepository,
    IJwtService jwtService)
    : IIdentityService
{
    public async Task<CreateUserResponse> RegisterAsync(CreateUserRequest request)
    {
        var applicationUser = await CreateApplicationUserAsync(request);

        await AssignDefaultRoleAsync(applicationUser);

        var user = await CreateUser(applicationUser, request);

        await SignInUserAsync(applicationUser);

        var token = await GenerateJwtTokenAsync(applicationUser, user.Id);

        return new CreateUserResponse
        {
            Email = user.Email,
            Plan = user.Plan,
            Token = token
        };
    }

    private async Task<User> CreateUser(ApplicationUser applicationUser, CreateUserRequest request)
    {
        var user = User.Create(applicationUser.Id, request.Email);

        await userRepository.AddAsync(user);

        return user;
    }

    private async Task AssignDefaultRoleAsync(ApplicationUser user)
    {
        var roleResult = await userManager.AddToRoleAsync(user, Roles.User);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));

            await userManager.DeleteAsync(user);

            throw new InvalidOperationException($"Role assignment failed: {errors}");
        }
    }

    private async Task<ApplicationUser> CreateApplicationUserAsync(CreateUserRequest request)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(applicationUser, request.Password);

        if (result.Succeeded) return applicationUser;

        var errors = string.Join("; ", result.Errors.Select(e => e.Description));

        throw new InvalidOperationException($"Registration failed for user {request.Email} with errors {errors}");
    }

    private async Task SignInUserAsync(ApplicationUser user) =>
        await signInManager.SignInAsync(user, isPersistent: true);

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, Guid userId)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new Dictionary<string, string>();

        if (roles.Any())
            claims.Add(ClaimTypes.Role, string.Join(", ", roles));

        var token = jwtService.GenerateToken(userId.ToString(), claims);

        return token.Value;
    }
}