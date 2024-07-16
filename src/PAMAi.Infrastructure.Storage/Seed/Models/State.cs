using System.Text.Json.Serialization;

namespace PAMAi.Infrastructure.Storage.Seed.Models;

internal sealed record State
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
