using Scalar.AspNetCore;
using UrlShortener.Application.UrlShortening.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTheme(ScalarTheme.DeepSpace)
            .WithBaseServerUrl("/api-dev");
    });
}

app.UseHttpsRedirection();

app.Run();