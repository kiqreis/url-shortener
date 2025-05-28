
namespace UrlShortener.Api.Common.Api;

public static class Endpoint
{
    public static void MapEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/v1");

        group.MapGroup("/short-urls")
            .WithTags("ShortUrls");
    }
}

