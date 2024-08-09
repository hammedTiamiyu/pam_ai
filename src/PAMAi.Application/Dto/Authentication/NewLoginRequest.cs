using FluentValidation;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Dto.Authentication;

public record NewLoginRequest: LoginRequest
{
    /// <summary>
    /// Role to sign in as.
    /// </summary>
    /// <example>1</example>
    public ApplicationRole Role { get; set; }
}

internal class NewLoginRequestValidator: AbstractValidator<NewLoginRequest>
{
    public NewLoginRequestValidator()
    {
        RuleFor(l => l.Username).NotEmpty();
        RuleFor(l => l.Password).NotEmpty();
        RuleFor(l => l.Role).IsInEnum();
    }
}