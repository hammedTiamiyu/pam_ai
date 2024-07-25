using FluentValidation;

namespace PAMAi.Application.Dto.Account;

public record LogoutRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

internal class LogoutRequestValidator: AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(l => l.AccessToken).NotEmpty();
        RuleFor(l => l.RefreshToken).NotEmpty();
    }
}