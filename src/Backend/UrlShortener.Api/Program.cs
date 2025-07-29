using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using UrlShortener.Api.Common.Api;
using UrlShortener.Application.Cache.Services;
using UrlShortener.Application.Common;
using UrlShortener.Application.Serialization;
using UrlShortener.Application.UrlShortening.Services;
using UrlShortener.Application.Users.Services;
using UrlShortener.Application.Common.Security;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Cache;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Infrastructure.Identity;
using UrlShortener.Infrastructure.Identity.Extensions;
using UrlShortener.Infrastructure.Identity.Handlers;
using UrlShortener.Infrastructure.Identity.Services;
using UrlShortener.Infrastructure.Repositories;
using UrlShortener.Infrastructure.Security;
using UrlShortener.Infrastructure.Serialization;

var builder = WebApplication.CreateBuilder(args);

var redisConnection = builder.Configuration.GetConnectionString("Redis");
var redis = ConnectionMultiplexer.Connect(redisConnection);
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? new JwtConfig();

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Url Shortener",
        Version = "v1"
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            @"Jwt Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in text input below. Example: 'Bearer JwtToken'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IJsonSerializer, JsonSerializerImpl>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<CookieAuthService>();
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddScoped<JwtTokenHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterApplicationUserHandler>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<RoleAssignmentHandler>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IBase58Encoder, Base58Encoder>();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("JwtConfig")
);

builder.Services.AddSingleton(options => options.GetRequiredService<IOptions<JwtConfig>>().Value);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:518", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfig.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["authToken"];

            if (!string.IsNullOrWhiteSpace(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    }).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

var app = builder.Build();

var config = app.Services.GetRequiredService<IConfiguration>();

if (config.GetValue<bool>("Seed:EnableSeed"))
    await app.Services.SeedRolesAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();