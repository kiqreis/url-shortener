using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.DTOs.Responses;
using UrlShortener.Application.Users.Services;
using UrlShortener.Domain.Common.Security;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Identity.Constants;
using UrlShortener.Infrastructure.Identity.Handlers;

namespace UrlShortener.Infrastructure.Identity.Services;

public class IdentityService(
    RegisterApplicationUserHandler  registerApplicationUserHandler,
    RoleAssignmentHandler  roleAssignmentHandler,
    RegisterUserHandler  registerUserHandler,
    AuthenticationHandler  authenticationHandler,
    JwtTokenHandler  jwtTokenHandler)
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
            Token = token
        };
    }
}