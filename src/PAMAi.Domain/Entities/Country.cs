using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Country.
/// </summary>
public sealed class Country: IEntity<int>, IEquatable<Country>
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

    public bool Equals(Country? x, Country? y)
    {
        if (x is null && y is null)
            return true;

        return
           string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(Country? other)
    {
        if (other is null)
            return false;

        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Country);

    public override int GetHashCode() => Name.GetHashCode();
}
