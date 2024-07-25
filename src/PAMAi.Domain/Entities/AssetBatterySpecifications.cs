namespace PAMAi.Domain.Entities;

/// <summary>
/// Battery specifications for an asset.
/// </summary>
public class AssetBatterySpecifications
{
    /// <summary>
    /// Total energy storage capacity of the battery.
    /// </summary>
    public double TotalCapacity { get; set; }

    /// <summary>
    /// Amount of energy that can be effectively used.
    /// </summary>
    public double? UsableCapacity { get; set; }

    /// <summary>
    /// Technology of the battery (e.g., lithium-ion, lead-acid).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Nominal voltage of the battery system.
    /// </summary>
    public double Voltage { get; set; }

    /// <summary>
    /// Percentage of the battery that has been discharged.
    /// </summary>
    public double? DepthOfDischarge { get; set; }

    /// <summary>
    /// Number of charge/discharge cycles before capacity reduction.
    /// </summary>
    public int? CycleLife { get; set; }

    /// <summary>
    /// Rate at which the battery can be charged or discharged.
    /// </summary>
    public double? ChargeDischargeRate { get; set; }

    /// <summary>
    /// Efficiency of the battery in storing and delivering energy.
    /// </summary>
    public double? Efficiency { get; set; }

    /// <summary>
    /// Features for monitoring battery health and managing charge/discharge.
    /// </summary>
    public string? ManagementSystem { get; set; }

    /// <summary>
    /// Warranty period and expected lifespan of the battery.
    /// </summary>
    public string? WarrantyAndLifespan { get; set; }
}
