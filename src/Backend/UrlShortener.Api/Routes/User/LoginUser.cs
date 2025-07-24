using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.Services;

namespace UrlShortener.Api.Routes.User;

public class LoginUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapPost("/login", HandleAsync)
        .WithName("User: user login")
        .WithDescription("Performs the user login")
        .WithOrder(3)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync([FromBody] LoginRequest request, [FromServices] IIdentityService handler)
    {
        var result = await handler.LoginAsync(request);

        return Results.Ok(result);    
    }
}
