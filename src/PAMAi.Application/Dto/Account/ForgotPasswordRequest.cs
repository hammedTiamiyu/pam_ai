using FluentValidation;

namespace PAMAi.Application.Dto.Account;

public record ForgotPasswordRequest
{
    /// <summary>
    /// Account email
    /// </summary>
    /// <example>johndoe@gmail.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL of the reset password screen
    /// </summary>
    /// <example>https://google.com</example>
    public string CallbackUrl { get; set; } = string.Empty;
}

internal class ForgotPasswordRequestValidator: AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.CallbackUrl).NotNull();
    }
}
