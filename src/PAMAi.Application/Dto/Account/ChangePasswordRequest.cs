using FluentValidation;

namespace PAMAi.Application.Dto.Account;

public record ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

internal class ChangePasswordRequestValidator: AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEqual(x => x.OldPassword, StringComparer.Ordinal).WithMessage("New password must be different from the old one.");
    }
}