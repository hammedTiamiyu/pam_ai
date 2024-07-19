using System.Text.Json.Serialization;

namespace PAMAi.Application.Services.Models.Internal;
internal class CountryApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("states")]
    public List<StateApiResponse> States { get; set; } = [];
}
