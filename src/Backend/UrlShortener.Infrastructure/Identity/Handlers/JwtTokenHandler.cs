using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrlShortener.Application.Common.Security;

namespace UrlShortener.Infrastructure.Identity.Handlers;

public class JwtTokenHandler(UserManager<ApplicationUser> userManager, IJwtService jwtService, ILogger<JwtTokenHandler> logger)
{
    public async Task<JwtTokenResult> GenerateJwtTokenAsync(ApplicationUser user, Guid userId)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new Dictionary<string, string>();

        if (roles.Any())
            claims.Add(ClaimTypes.Role, string.Join(",", roles));

        var token = jwtService.GenerateToken(userId.ToString(), claims);

        logger.LogInformation("JWT token generated successfully for user: {UserId}", userId);
        
        return token;
    }   
}