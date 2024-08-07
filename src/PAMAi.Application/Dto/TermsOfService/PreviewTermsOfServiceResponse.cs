namespace PAMAi.Application.Dto.TermsOfService;

public record PreviewTermsOfServiceResponse
{
    /// <example>1</example>
    public int Id { get; set; }

    /// <example>1.0</example>
    public double Version { get; set; }

    public DateTimeOffset CreatedUtc { get; set; }

    public DateTimeOffset EffectiveFromUtc { get; set; }

    /// <example>true</example>
    public bool IsActive => DateTimeOffset.UtcNow >= EffectiveFromUtc && !DeactivatedUtc.HasValue;

    public DateTimeOffset? DeactivatedUtc { get; set; }
}
