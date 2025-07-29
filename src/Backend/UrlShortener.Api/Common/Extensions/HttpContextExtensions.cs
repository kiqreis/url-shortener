using UrlShortener.Infrastructure.Security;

namespace UrlShortener.Api.Common.Extensions;

public static class HttpContextExtensions
{
    public static void SetAuthToken(this HttpContext httpContext, string token)
    {
        var cookieService = httpContext.RequestServices.GetRequiredService<CookieAuthService>();

        cookieService.SetAuthCookie(httpContext, token);
    }

    public static void ClearAuthToken(this HttpContext httpContext)
    {
        var cookieService = httpContext.RequestServices.GetRequiredService<CookieAuthService>();

        cookieService.RemoveAuthCookie(httpContext);
    }
}