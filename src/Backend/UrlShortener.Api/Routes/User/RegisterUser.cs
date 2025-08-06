using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Api.Common.Extensions;
using UrlShortener.Application.Users.DTOs.Requests;
using UrlShortener.Application.Users.Services;

namespace UrlShortener.Api.Routes.User;

public class RegisterUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapPost("/register", HandleAsync)
        .WithName("User: create a user")
        .WithDescription("Create a new user")
        .WithOrder(1)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync([FromBody] CreateUserRequest request,
        [FromServices] IIdentityService handler, HttpContext httpContext)
    {
        var result = await handler.RegisterAsync(request);

        httpContext.SetAuthToken(result.Token);

        return Results.Created($"{result.Email}", result);
    }
}