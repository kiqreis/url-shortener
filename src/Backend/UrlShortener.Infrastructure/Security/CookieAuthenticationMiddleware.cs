using Microsoft.AspNetCore.Http;

namespace UrlShortener.Infrastructure.Security;

public class CookieAuthenticationMiddleware(RequestDelegate next, JwtService jwtService)
{
    public async Task? InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies["authToken"];

        if (!string.IsNullOrWhiteSpace(token))
        {
            var validation = jwtService.ValidateToken(token);

            if (validation.IsValid)
            {
                context.User = validation.Principal;
            }
            else
            {
                context.Response.Cookies.Delete("authToken");
            }
        }

        await next(context);
    }
}