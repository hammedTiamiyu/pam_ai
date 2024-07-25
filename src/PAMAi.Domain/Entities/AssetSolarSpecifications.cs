using PAMAi.Domain.Enums;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Solar panel specifications of an asset.
/// </summary>
public class AssetSolarSpecifications
{
    /// <summary>
    /// Indicates the presence of nearby structures or natural features that
    /// could cause shading.
    /// </summary>
    public bool? PotentialShading { get; set; }

    /// <summary>
    /// Type of solar panels used (e.g., thin film, monocrystalline, polycrystalline).
    /// </summary>
    public string? PanelType { get; set; }

    /// <summary>
    /// Total wattage (W) capacity of the solar installation.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Efficiency rating of the solar panels.
    /// </summary>
    public int EfficiencyRating { get; set; }

    /// <summary>
    /// Total count of solar panels in the installation.
    /// </summary>
    public int PanelCount { get; set; }

    /// <summary>
    /// Time(s) of the day when the user maximises their energy usage.
    /// </summary>
    public EnergyUsageTimes? PeakEnergyUsageTime { get; set; }

    /// <summary>
    /// Energy consumption habit or pattern of the user.
    /// </summary>
    public EnergyConsumptionHabit? EnergyConsumptionHabit { get; set; }

    /// <summary>
    /// Energy generated daily by the panels.
    /// </summary>
    public double? EnergyGeneratedDaily { get; set; }

    /// <inheritdoc cref="Enums.EnergyUsageFlexibility"/>
    public EnergyUsageFlexibility? EnergyUsageFlexibility { get; set; }
}
