using Microsoft.AspNetCore.Identity;
using UrlShortener.Infrastructure.Identity.Constants;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class RoleAssignmentHandler(UserManager<ApplicationUser> userManager)
{
    public async Task AssignDefaultRoleAsync(ApplicationUser user)
    {
        var roleResult = await userManager.AddToRoleAsync(user, Roles.User);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

            await userManager.DeleteAsync(user);
            
            throw new InvalidOperationException($"Role assignment failed: {errors}");
        }
    }
}