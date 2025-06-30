using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UrlShortener.Domain.Common.Security;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace UrlShortener.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly JwtConfig _jwtConfig;

    public JwtService(JwtConfig jwtConfig)
    {
        _jwtConfig = jwtConfig;
        ValidateConfig();
    }

    private void ValidateConfig()
    {
        if (string.IsNullOrWhiteSpace(_jwtConfig.Secret))
            throw new ArgumentException("Secret cannot be null or empty", nameof(_jwtConfig.Secret));

        if (string.IsNullOrWhiteSpace(_jwtConfig.Issuer))
            throw new ArgumentException("Issuer cannot be null or empty", nameof(_jwtConfig.Issuer));

        if (string.IsNullOrWhiteSpace(_jwtConfig.Audience))
            throw new ArgumentException("Audience cannot be null or empty", nameof(_jwtConfig.Audience));

        if (_jwtConfig.ExpiryInMinutes <= 0)
            throw new ArgumentException("ExpiryInMinutes cannot be negative", nameof(_jwtConfig.ExpiryInMinutes));
    }

    private SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

    public JwtToken GenerateToken(string subject, Dictionary<string, string>? customClaims = null)
    {
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Subject cannot be null or empty", nameof(subject));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        if (customClaims?.Count > 0)
            claims.AddRange(customClaims.Select(k => new Claim(k.Key, k.Value)));

        var securityKey = GetSymmetricSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var validFrom = DateTime.UtcNow;
        var validTo = validFrom.AddMinutes(_jwtConfig.ExpiryInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            notBefore: validFrom,
            expires: validTo,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValue = tokenHandler.WriteToken(token);
        var claimsDict = customClaims ?? new Dictionary<string, string>();

        return new JwtToken
        {
            Value = tokenValue,
            ValidFrom = validFrom,
            ValidTo = validTo,
            Subject = subject,
            Issuer = _jwtConfig.Issuer,
            Claims = claimsDict
        };
    }

    private JwtTokenValidation ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var claims = principal.Claims
                .Where(c => c.Type != JwtRegisteredClaimNames.Sub &&
                            c.Type != JwtRegisteredClaimNames.Jti &&
                            c.Type != JwtRegisteredClaimNames.Iat &&
                            c.Type != JwtRegisteredClaimNames.Iss &&
                            c.Type != JwtRegisteredClaimNames.Aud &&
                            c.Type != JwtRegisteredClaimNames.Exp &&
                            c.Type != JwtRegisteredClaimNames.Nbf)
                .ToDictionary(c => c.Type, c => c.Value);

            var subject = principal.Identity?.Name ??
                          principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

            return JwtTokenValidation.Success(principal, subject, claims);
        }
        catch (SecurityTokenExpiredException)
        {
            return JwtTokenValidation.Failure("Token has expired");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return JwtTokenValidation.Failure("Expired signature");
        }
        catch (SecurityTokenValidationException ex)
        {
            return JwtTokenValidation.Failure($"Invalid token: {ex.Message}");
        }
        catch (Exception ex)
        {
            return JwtTokenValidation.Failure($"Error token validation: {ex.Message}");
        }
    }
    
    public JwtToken? RefreshToken(string token, int? newExpiryMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        
        var validationResult = ValidateToken(token);
        
        if (!validationResult.IsValid || string.IsNullOrEmpty(validationResult.Subject))
            return null;

        return GenerateToken(validationResult.Subject, validationResult.Claims);
    }
}