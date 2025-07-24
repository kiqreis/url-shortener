using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.DTOs.Responses;

namespace UrlShortener.Application.Users.Services;

public interface IIdentityService
{
    Task<CreateUserResponse> RegisterAsync(CreateUserRequest request);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
}