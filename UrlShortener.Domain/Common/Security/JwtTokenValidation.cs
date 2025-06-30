using System.Security.Claims;

namespace UrlShortener.Domain.Common.Security;

public class JwtTokenValidation
{
    public bool IsValid { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string> Claims { get; set; } = new();
    public string? Subject { get; set; }

    public static JwtTokenValidation Success(ClaimsPrincipal principal, string subject,
        Dictionary<string, string> claims)
        => new()
        {
            IsValid = true,
            Principal = principal,
            Subject = subject,
            Claims = claims,
        };

    public static JwtTokenValidation Failure(string errorMessage) => new()
    {
        IsValid = false,
        ErrorMessage = errorMessage,
    };
}