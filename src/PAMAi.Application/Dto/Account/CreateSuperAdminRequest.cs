using FluentValidation;

namespace PAMAi.Application.Dto.Account;

public record CreateSuperAdminRequest
{
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;

    /// <example>JohnDoe</example>
    public string Username { get; set; } = string.Empty;

    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <example>John</example>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <example>1</example>
    public long StateId { get; set; }

    /// <example>p@55woRd</example>
    public string Password { get; set; } = string.Empty;

    /// <example>p@55woRd</example>
    public string PasswordConfirmation { get; set; } = string.Empty;

    /// <example>true</example>
    public bool AcceptTermsOfService { get; set; }
}

internal class CreateSuperAdminRequestValidator: AbstractValidator<CreateSuperAdminRequest>
{
    public CreateSuperAdminRequestValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
        RuleFor(c => c.Username).NotEmpty();
        RuleFor(c => c.Email).EmailAddress();
        RuleFor(c => c.Password).Equal(c => c.PasswordConfirmation);
        RuleFor(c => c.AcceptTermsOfService).Equal(true).WithMessage("Super Admin must accept the Terms of Service to create an account on this platform.");
    }
}