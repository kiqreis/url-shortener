using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.DTOs.Responses;
using UrlShortener.Application.Users.Services;
using UrlShortener.Infrastructure.Identity.Handlers;

namespace UrlShortener.Infrastructure.Identity.Services;

public class IdentityService(
    RegisterApplicationUserHandler  registerApplicationUserHandler,
    RoleAssignmentHandler  roleAssignmentHandler,
    RegisterUserHandler  registerUserHandler,
    AuthenticationHandler  authenticationHandler,
    JwtTokenHandler  jwtTokenHandler,
    LoginHandler loginHandler,
    IUserService userService)
    : IIdentityService
{
    public async Task<CreateUserResponse> RegisterAsync(CreateUserRequest request)
    {
        var applicationUser = await registerApplicationUserHandler.CreateUserAsync(request);
        
        await roleAssignmentHandler.AssignDefaultRoleAsync(applicationUser);

        var user = await registerUserHandler.CreateUserAsync(applicationUser, request);

        await authenticationHandler.SignUserAsync(applicationUser);

        var token = await jwtTokenHandler.GenerateJwtTokenAsync(applicationUser, user.Id);

        return new CreateUserResponse
        {
            Email = user.Email,
            Plan = user.Plan,
            Token = token.Value
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var applicationUser = await loginHandler.ValidateCredentialsAsync(request);

        if (applicationUser is null) return null;

        var user = await userService.GetByEmailAsync(request.Email);

        if (user is null) return null;

        var jwtToken = await jwtTokenHandler.GenerateJwtTokenAsync(applicationUser, applicationUser.Id);

        return new LoginResponse
        {
            Email = user.Email,
            Plan = user.Plan,
            Token = jwtToken.Value,
            TokenExpiry = jwtToken.ValidTo
        };
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }
}