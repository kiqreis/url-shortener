using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.Users.Services;

namespace UrlShortener.Api.Routes.User;

public class GetUserByEmail : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapGet("/{email}", HandleAsync)
        .RequireAuthorization()
        .WithName("User: get user")
        .WithDescription("Get user by email")
        .WithOrder(2)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync([FromRoute] string email, [FromServices] IUserService handler)
    {
        var result = await handler.GetByEmailAsync(email);

        return Results.Ok(result);
    }
}
