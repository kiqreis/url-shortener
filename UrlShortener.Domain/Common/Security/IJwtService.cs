namespace UrlShortener.Domain.Common.Security;

public interface IJwtService
{
    JwtToken GenerateToken(string subject, Dictionary<string, string>? customClaims = null);
}