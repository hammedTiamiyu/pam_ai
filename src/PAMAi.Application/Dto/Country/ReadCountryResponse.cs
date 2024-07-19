namespace PAMAi.Application.Dto.Country;

public record ReadCountryResponse
{
    /// <example>1</example>
    public int Id { get; set; }

    /// <example>Nigeria</example>
    public string Name { get; set; } = string.Empty;
}
