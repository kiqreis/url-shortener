
using UrlShortener.Api.Routes.UrlShortening;
using UrlShortener.Api.Routes.User;

namespace UrlShortener.Api.Common.Api;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1");

        group.MapGroup("/urls")
            .WithTags("ShortUrls")
            .MapEndpoint<CreateShortUrl>()
            .MapEndpoint<RedirectToOriginalUrl>();

        group.MapGroup("/users")
            .WithTags("Users")
            .MapEndpoint<RegisterUser>()
            .MapEndpoint<GetUserByEmail>();
    }

    private static IEndpointRouteBuilder MapEndpoint<T>(this IEndpointRouteBuilder routeBuilder) where T : IEndpoint
    {
        T.Map(routeBuilder);
        return routeBuilder;
    }
}