using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.Services;

namespace UrlShortener.Api.Routes;

public class RedirectToOriginalUrl : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapGet("/{shortCode}", HandleAsync)
        .WithName("ShortUrl: redirect url")
        .WithDescription("Redirects to the original URL")
        .WithOrder(2)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync(HttpContext context, [FromServices] IUrlShorteningService handler,
        [FromRoute] string shortCode)
    {
        var request = new TrackUrlRequest(
            shortCode,
            context.Connection.RemoteIpAddress!.ToString(),
            context.Request.Headers.Referer,
            context.Request.Headers.UserAgent
        );

        var shortUrl = await handler.GetAndTrackAsync(request);

        return Results.Redirect(shortUrl.OriginalUrl, permanent: true);
    }
}