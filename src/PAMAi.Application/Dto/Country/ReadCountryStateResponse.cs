namespace PAMAi.Application.Dto.Country;
public record ReadCountryStateResponse
{
    /// <example>13</example>
    public long Id { get; set; }

    /// <example>Lagos</example>
    public string Name { get; set; } = string.Empty;
}
