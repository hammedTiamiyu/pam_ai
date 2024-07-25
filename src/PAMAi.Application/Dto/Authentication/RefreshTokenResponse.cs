namespace PAMAi.Application.Dto.Authentication;
public record RefreshTokenResponse
{
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
    public string AccessToken { get; set; } = string.Empty;

    public DateTimeOffset Expires { get; set; }

    /// <example>IiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9Jf36POk6c</example>
    public string RefreshToken { get; set; } = string.Empty;
}
