using System.Text.Json.Serialization;

namespace PAMAi.Infrastructure.Storage.Seed.Models;

internal sealed record Country
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("states")]
    public List<State> States { get; set; } = [];
}
