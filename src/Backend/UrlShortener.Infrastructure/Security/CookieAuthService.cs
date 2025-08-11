using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Infrastructure.Security;

public class CookieAuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
{
    private readonly bool _isDevelopment =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    private CookieOptions GetAuthCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !_isDevelopment,
        Path = "/",
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("JwtConfig:ExpiryInMinutes"))
    };

    public void SetAuthCookie(string token)
    {
        httpContextAccessor.HttpContext.Response.Cookies.Append("authToken", token, GetAuthCookieOptions());
    }

    public void RemoveAuthCookie()
    {
        httpContextAccessor.HttpContext.Response.Cookies.Delete("authToken", new CookieOptions
        {
            Path = "/",
            Secure = !_isDevelopment,
            SameSite = _isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict
        });
    }

    public string? GetAuthToken() => httpContextAccessor.HttpContext.Request.Cookies["authToken"];
}