using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.Services;

namespace UrlShortener.Api.Routes;

public class CreateShortUrl : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapPost("/shorten", HandleAsync)
        .WithName("ShortUrl: shorten url")
        .WithDescription("Shorten a url")
        .WithOrder(1)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync(HttpContext context, [FromBody] ShortenUrlRequest request,
        [FromServices] IUrlShorteningService handler)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            ipAddress = forwardedFor.FirstOrDefault() ?? ipAddress;
        }

        request = request with { IpAddress = ipAddress ?? "unknown" };

        var result = await handler.ShortenUrlAsync(request);

        return Results.Created($"/{result.ShortCode}", result);
    }
}