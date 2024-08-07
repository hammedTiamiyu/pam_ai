using FluentValidation;

namespace PAMAi.Application.Dto.TermsOfService;
public record CreateTermsOfServiceRequest
{
    /// <example>10.4</example>
    public double Version { get; set; }

    /// <example>Sadipscing kasd aliquip accusam sea vel justo amet clita facilisis sanctus magna diam dolor hendrerit ea magna dolores ipsum.</example>
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset EffectiveFromUtc { get; set; }
}

internal class CreateTermsOfServiceValidator: AbstractValidator<CreateTermsOfServiceRequest>
{
    public CreateTermsOfServiceValidator()
    {
        RuleFor(x => x.Version).GreaterThan(0.0);

        RuleFor(x => x.Content).NotEmpty();
    }
}