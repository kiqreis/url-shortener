using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Infrastructure.Identity.Constants;

namespace UrlShortener.Infrastructure.Identity.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        string[] roles = [Roles.User, Roles.Admin];

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;

            var result = await roleManager.CreateAsync(new ApplicationRole
            {
                Name = role,
                NormalizedName = role.ToUpperInvariant()
            });

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}