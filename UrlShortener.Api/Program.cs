using Scalar.AspNetCore;
using StackExchange.Redis;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.Cache.Services;
using UrlShortener.Application.Common;
using UrlShortener.Application.Serialization;
using UrlShortener.Application.UrlShortening.Services;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Cache;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Serialization;

var builder = WebApplication.CreateBuilder(args);

var redisConnection = builder.Configuration.GetConnectionString("Redis");
var redis = ConnectionMultiplexer.Connect(redisConnection);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IJsonSerializer, JsonSerializerImpl>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

builder.Services.AddSingleton<IBase58Encoder, Base58Encoder>();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();