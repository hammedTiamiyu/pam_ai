using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Country state.
/// </summary>
public sealed class State: IEntity<long>
{
    /// <inheritdoc/>
    public long Id { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parent country's ID.
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Country navigation property.
    /// </summary>
    public Country? Country { get; set; }
}
