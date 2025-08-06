using Microsoft.AspNetCore.Identity;
using UrlShortener.Application.Users.DTOs.Requests;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class LoginHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
{
    public async Task<ApplicationUser?> ValidateCredentialsAsync(LoginRequest request)
    {
        var applicationUser = await userManager.FindByEmailAsync(request.Email);

        if (applicationUser is null) return null;

        var result =
            await signInManager.CheckPasswordSignInAsync(applicationUser, request.Password, lockoutOnFailure: false);

        return result.Succeeded ? applicationUser : null;
    }

    public async Task SignInUserAsync(ApplicationUser user, bool rememberMe = false) =>
        await signInManager.SignInAsync(user, isPersistent: rememberMe);

    public async Task SignOutUserAsync() => await signInManager.SignOutAsync();
}