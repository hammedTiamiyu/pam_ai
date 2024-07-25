using FluentValidation;

namespace PAMAi.Application.Dto.Authentication;

public record RefreshTokenRequest
{
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
    public string AccessToken { get; set; } = string.Empty;

    /// <example>IiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9Jf36POk6c</example>
    public string RefreshToken { get; set; } = string.Empty;
}

internal class RefreshTokenRequestValidator: AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(l => l.AccessToken).NotEmpty();
        RuleFor(l => l.RefreshToken).NotEmpty();
    }
}