using FluentValidation;
using PAMAi.Domain.Entities;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Dto.Account;
public record UpdateProfileRequest
{
    /// <example>Jane</example>
    public string FirstName { get; set; } = string.Empty;

    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;

    /// <example>Esse diam et in tempor commodo aliquyam in et placerat duis at ipsum et sanctus consequat dolore.</example>
    public string Bio { get; set; } = string.Empty;

    /// <example>2</example>
    public Gender? Gender { get; set; }

    /// <example>1</example>
    public string? HouseNumber { get; set; }

    /// <example>Calabar</example>
    public string? City { get; set; }

    /// <example>299</example>
    public long? StateId { get; set; }

    /// <example>null</example>
    public string? CompanyName { get; set; }

    public static TypeAdapterConfig ToUserProfile => TypeAdapterConfig<UpdateProfileRequest, UserProfile>
        .NewConfig()
        .Ignore(dest => dest.StateId!)
        .Config;
}

internal class UpdateProfileRequestValidator: AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Bio);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.StateId).GreaterThan(0).When(x => x.StateId != null);
    }
}