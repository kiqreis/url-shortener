
using UrlShortener.Api.Routes.UrlShortening;

namespace UrlShortener.Api.Common.Api;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1");

        group.MapGroup("/short-urls")
            .WithTags("ShortUrls")
            .MapEndpoint<CreateShortUrl>()
            .MapEndpoint<RedirectToOriginalUrl>();
    }

    private static IEndpointRouteBuilder MapEndpoint<T>(this IEndpointRouteBuilder routeBuilder) where T : IEndpoint
    {
        T.Map(routeBuilder);
        return routeBuilder;
    }
}

