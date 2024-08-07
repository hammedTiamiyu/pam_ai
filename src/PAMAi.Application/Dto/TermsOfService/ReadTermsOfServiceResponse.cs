namespace PAMAi.Application.Dto.TermsOfService;
public record ReadTermsOfServiceResponse
{
    /// <example>1</example>
    public int Id { get; set; }

    /// <example>2.1</example>
    public double Version { get; set; }

    /// <example>Sadipscing kasd aliquip accusam sea vel justo amet clita facilisis sanctus magna diam dolor hendrerit ea magna dolores ipsum.</example>
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedUtc { get; set; }

    public DateTimeOffset EffectiveFromUtc { get; set; }

    /// <example>true</example>
    public bool IsActive => DateTimeOffset.UtcNow >= EffectiveFromUtc && !DeactivatedUtc.HasValue;

    public DateTimeOffset? DeactivatedUtc { get; set; }
}
