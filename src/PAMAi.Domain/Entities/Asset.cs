using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Asset.
/// </summary>
public class Asset: IEntity<Guid>
{
    /// <inheritdoc/>
    public Guid Id { get; set; }

    /// <summary>
    /// Location of the installation.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Asset name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Size.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Installation date (in UTC).
    /// </summary>
    public DateTimeOffset InstallationDateUtc { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? GenerationLoad { get; set; }

    /// <summary>
    /// User ID.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Installer ID.
    /// </summary>
    public string InstallerId { get; set; } = string.Empty;

    /// <summary>
    /// Asset creation date (in UTC).
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; }

    /// <summary>
    /// Last date the asset was modified (in UTC).
    /// </summary>
    public DateTimeOffset? LastModifiedUtc { get; set; }

    /// <summary>
    /// Pricing details.
    /// </summary>
    public AssetPricingDetails PricingDetails { get; set; } = new();

    /// <summary>
    /// Solar specifications.
    /// </summary>
    public AssetSolarSpecifications SolarSpecifications { get; set; } = new();

    /// <summary>
    /// Inverter specifications.
    /// </summary>
    public AssetInverterSpecifications InverterSpecifications { get; set; } = new();

    /// <summary>
    /// Battery specifications.
    /// </summary>
    public AssetBatterySpecifications BatterySpecifications { get; set; } = new();
}
