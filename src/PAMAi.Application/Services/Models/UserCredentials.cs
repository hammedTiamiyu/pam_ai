namespace PAMAi.Application.Services.Models;

public record UserCredentials
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
