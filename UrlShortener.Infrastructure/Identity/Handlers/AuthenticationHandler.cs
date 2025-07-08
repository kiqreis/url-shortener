using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class AuthenticationHandler(SignInManager<ApplicationUser> signInManager)
{
    public async Task SignUserAsync(ApplicationUser user) => await signInManager.SignInAsync(user, true);
}