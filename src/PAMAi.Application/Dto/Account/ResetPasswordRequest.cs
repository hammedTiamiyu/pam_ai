using FluentValidation;

namespace PAMAi.Application.Dto.Account;

public record ResetPasswordRequest
{
    /// <summary>
    /// Account email
    /// </summary>
    /// <example>johndoe@gmail.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Reset password token
    /// </summary>
    /// <example>CfDJ8H0qIUqBN/NNqOxUg89rh/</example>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    /// <example>AH@RdPa55word</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password. Must match the new password
    /// </summary>
    /// <example>AH@RdPa55word</example>
    public string ConfirmPassword { get; set; } = string.Empty;
}

internal class ResetPasswordRequestValidator: AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Token).NotNull();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage($"{nameof(ResetPasswordRequest.ConfirmPassword)} must match {nameof(ResetPasswordRequest.Password)}.");
    }
}
