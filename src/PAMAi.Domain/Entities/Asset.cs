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
    /// Asset owner's profile ID.
    /// </summary>
    public Guid OwnerProfileId { get; set; }

    /// <summary>
    /// Installer profile ID.
    /// </summary>
    public Guid InstallerProfileId { get; set; }

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

    public UserProfile OwnerProfile { get; set; } = new();

    public UserProfile InstallerProfile { get; set; } = new();
}
