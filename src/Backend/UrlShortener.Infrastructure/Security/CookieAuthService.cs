using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Infrastructure.Security;

public class CookieAuthService(IConfiguration configuration)
{
    private CookieOptions GetAuthCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        Path = "/",
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("ExpiryInMinutes"))
    };

    public void SetAuthCookie(HttpContext httpContext, string token)
    {
        httpContext.Response.Cookies.Append("authToken", token, GetAuthCookieOptions());
    }

    public void RemoveAuthCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("authToken", new CookieOptions
        {
            Path = "/",
            Secure = true
        });
    }
}