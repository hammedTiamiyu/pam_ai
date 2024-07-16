namespace PAMAi.Application.Services.Models;

public record Tokens
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset Expires { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
