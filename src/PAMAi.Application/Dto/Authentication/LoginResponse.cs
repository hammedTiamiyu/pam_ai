namespace PAMAi.Application.Dto.Authentication;
public record LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset Expires { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
