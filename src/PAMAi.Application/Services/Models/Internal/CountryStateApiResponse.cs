using System.Text.Json.Serialization;

namespace PAMAi.Application.Services.Models.Internal;
internal sealed record CountryStateApiResponse
{
    [JsonPropertyName("error")]
    public bool HasError { get; set; }

    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<CountryApiResponse> Countries { get; set; } = [];
}
