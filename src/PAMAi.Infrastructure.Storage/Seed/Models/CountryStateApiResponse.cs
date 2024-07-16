using System.Text.Json.Serialization;

namespace PAMAi.Infrastructure.Storage.Seed.Models;
internal sealed record CountryStateApiResponse
{
    [JsonPropertyName("error")]
    public bool HasError { get; set; }

    [JsonPropertyName("msg")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<Country> Countries { get; set; } = [];
}
