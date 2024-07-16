using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Country state.
/// </summary>
public sealed class State: IEntity<long>, IEquatable<State>
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

    public bool Equals(State? other)
    {
        if (other is null)
            return false;

        return
            string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
            CountryId == other.CountryId;
    }

    public override bool Equals(object? obj) => Equals(obj as State);

    public override int GetHashCode() => (Name, CountryId).GetHashCode();
}
