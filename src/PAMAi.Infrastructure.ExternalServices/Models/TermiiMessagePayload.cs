using System.Text.Json.Serialization;

namespace PAMAi.Infrastructure.ExternalServices.Models;

internal class TermiiMessagePayload
{
    internal required string To { get; set; } = string.Empty;
    internal required string Sms { get; set; } = string.Empty;
    internal required string From { get; set; } = string.Empty;
    internal required string Type { get; set; } = string.Empty;
    [JsonPropertyName("api_key")]
    internal required string ApiKey { get; set; } = string.Empty;
    internal required string Channel { get; set; } = string.Empty;
}
