using System.Text.Json;
using UrlShortener.Application.Serialization;

namespace UrlShortener.Infrastructure.Serialization;

public class JsonSerializerImpl : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, _options);

    public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options)!;
}