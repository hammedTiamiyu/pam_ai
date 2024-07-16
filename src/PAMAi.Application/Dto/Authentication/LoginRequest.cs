namespace PAMAi.Application.Dto.Authentication;

public record LoginRequest
{
    /// <summary>
    /// Account username or email.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Account password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
