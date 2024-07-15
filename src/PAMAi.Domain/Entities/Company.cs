using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Company containing assets in the application.
/// </summary>
public sealed class Company: IEntity<Guid>
{
    /// <inheritdoc/>
    public Guid Id { get; set; }

    /// <summary>
    /// Company name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Installers and engineers in the company.
    /// </summary>
    public List<UserProfile> UserProfiles { get; set; } = [];
}
