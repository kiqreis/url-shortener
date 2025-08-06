using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Api.Common.Extensions;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.Services;

namespace UrlShortener.Api.Routes.User;

public class LoginUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapPost("/login", HandleAsync)
        .WithName("User: user login")
        .WithDescription("Responsible for the user login")
        .WithOrder(3)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync([FromBody] LoginRequest request,
        [FromServices] IIdentityService handler, HttpContext httpContext)
    {
        var result = await handler.LoginAsync(request);

        if (result is null)
            return Results.Unauthorized();

        httpContext.SetAuthToken(result.Token);

        return Results.Ok(result);
    }
}