using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class LoginHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
{
    public async Task<ApplicationUser?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null) return null;

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        return result.Succeeded ? user : null;
    }

    public async Task SignInUserAsync(ApplicationUser user, bool rememberMe = false) =>
        await signInManager.SignInAsync(user, isPersistent: rememberMe);
}