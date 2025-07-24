namespace UrlShortener.Application.Common.Security;

public interface IJwtService
{
    JwtTokenResult GenerateToken(string subject, Dictionary<string, string>? customClaims = null);
}