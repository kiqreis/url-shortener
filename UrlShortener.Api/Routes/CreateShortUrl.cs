using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.UrlShortening.DTOs.Requests;
using UrlShortener.Application.UrlShortening.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Api.Routes;

public class CreateShortUrl : IEndpoint
{
    public static void Map(IEndpointRouteBuilder routeBuilder) => routeBuilder.MapPost("/shorten", HandleAsync)
        .WithName("ShortUrls: shorten url")
        .WithDescription("Shorten a url")
        .WithOrder(1)
        .WithOpenApi();

    private static async Task<IResult> HandleAsync([FromBody] ShortenUrlRequest request, [FromServices] IUrlShorteningService handler)
    {
        var result = await handler.ShortenUrlAsync(request);

        return Results.Created($"/{result.ShortCode}", result);
    }
}
