namespace PAMAi.Domain.Entities.Base;

/// <summary>
/// Keyless database entity.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Database entity having a primary key.
/// </summary>
/// <typeparam name="TKey">
/// Data type of the primary key.
/// </typeparam>
public interface IEntity<TKey>: IEntity
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public TKey Id { get; set; }
}
