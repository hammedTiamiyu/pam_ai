using FluentValidation;

namespace PAMAi.Application.Dto.Authentication;

public record LoginRequest
{
    /// <summary>
    /// Account username or email.
    /// </summary>
    /// <example>JohnDoe</example>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Account password.
    /// </summary>
    /// <example>AH@rDpa55W0rD</example>
    public string Password { get; set; } = string.Empty;
}

internal class LoginRequestValidator: AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(l => l.Username).NotEmpty();
        RuleFor(l => l.Password).NotEmpty();
    }
}