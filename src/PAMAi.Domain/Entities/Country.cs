using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Country.
/// </summary>
public sealed class Country: IEntity<int>
{
    /// <inheritdoc/>
    public int Id { get; set; }

    /// <summary>
    /// Name of country.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// States in the country.
    /// </summary>
    public List<State> States { get; set; } = [];
}
