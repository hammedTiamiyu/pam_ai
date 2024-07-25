using System.Text.Json.Serialization;

namespace PAMAi.Application.Services.Models.Internal;
internal class StateApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
