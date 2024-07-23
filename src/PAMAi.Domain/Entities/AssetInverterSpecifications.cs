using PAMAi.Domain.Enums;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Inverter specifications of an asset.
/// </summary>
public class AssetInverterSpecifications
{
    /// <summary>
    /// Maximum output capacity of the inverter (in kW/kVA).
    /// </summary>
    public double Capacity { get; set; }

    /// <summary>
    /// Efficiency in converting DC to AC.
    /// </summary>
    public double Efficiency { get; set; }

    /// <summary>
    /// Type of inverter (e.g., string, microinverters, hybrid).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Lower limit of the input voltage (V).
    /// </summary>
    public double InputVoltageLow { get; set; }

    /// <summary>
    /// Higher limit of the input voltage (V).
    /// </summary>
    public double InputVoltageHigh { get; set; }

    /// <summary>
    /// Number and capacity of MPPT channels.
    /// </summary>
    public string? MpptChannels { get; set; }

    /// <summary>
    /// Inverter system design.
    /// </summary>
    public InverterSystemDesign? SystemDesign { get; set; }

    /// <summary>
    /// Features for remote monitoring and management.
    /// </summary>
    public string? CommunicationFeatures { get; set; }

    /// <summary>
    /// Safety features like overvoltage, short circuit, and ground fault protection.
    /// </summary>
    public string? ProtectionFeatures { get; set; }

    /// <summary>
    /// Compliance with standards and certifications.
    /// </summary>
    public string? Certifications { get; set; }
}
