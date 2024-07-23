using PAMAi.Domain.Enums;

namespace PAMAi.Application.Dto.Asset;

public record CreateAssetRequest
{
    /// <example>=Plot 2322 Herbert Macaulay Way, Wuse, Abuja</example>
    public string? Location { get; set; }

    /// <example>Asset name</example>
    public string Name { get; set; } = string.Empty;

    /// <example>john.doe@gmail.com</example>
    public string Email { get; set; } = string.Empty;

    /// <example>skywne@T9Ikf</example>
    public string Password { get; set; } = string.Empty;

    /// <example>+2347010000000</example>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <example>John Doe</example>
    public string OwnerName { get; set; } = string.Empty;

    /// <example>3000 sqm.</example>
    public string? Size { get; set; }

    public DateTimeOffset InstallationDateUtc { get; set; }

    /// <example>null</example>
    public string? GenerationLoad { get; set; }

    public CreateAssetPricingDetailsRequest PricingDetails { get; set; } = new();

    public CreateAssetSolarSpecificationsRequest SolarSpecifications { get; set; } = new();

    public CreateAssetInverterSpecificationsRequest InverterSpecifications { get; set; } = new();

    public CreateAssetBatterySpecificationsRequest BatterySpecifications { get; set; } = new();

    public record CreateAssetPricingDetailsRequest
    {
        /// <example>1</example>
        public AssetPaymentPlan PaymentPlan { get; set; }

        /// <example>2</example>
        public AssetPricingPlan PricingPlan { get; set; }

        /// <example>650000.00</example>
        public decimal AmountPaid { get; set; }
    }

    public record CreateAssetSolarSpecificationsRequest
    {
        /// <example>true</example>
        public bool? PotentialShading { get; set; }

        /// <example>Thin film</example>
        public string? PanelType { get; set; }

        /// <example>3000</example>
        public int Size { get; set; }

        /// <example>98</example>
        public int EfficiencyRating { get; set; }

        /// <example>4</example>
        public int PanelCount { get; set; }

        /// <example>2</example>
        public EnergyUsageTimes? PeakEnergyUsageTime { get; set; }

        /// <example>null</example>
        public EnergyConsumptionHabit? EnergyConsumptionHabit { get; set; }

        /// <example>10000.00</example>
        public double? EnergyGeneratedDaily { get; set; }

        /// <example>1</example>
        public EnergyUsageFlexibility? EnergyUsageFlexibility { get; set; }
    }

    public record CreateAssetInverterSpecificationsRequest
    {
        /// <example>10000.00</example>
        public double Capacity { get; set; }

        /// <example>99.96</example>
        public double Efficiency { get; set; }

        /// <example>Microinverter</example>
        public string? Type { get; set; }

        /// <example>140</example>
        public double InputVoltageLow { get; set; }

        /// <example>275</example>
        public double InputVoltageHigh { get; set; }

        /// <example>null</example>
        public string? MpptChannels { get; set; }

        /// <example>2</example>
        public InverterSystemDesign? SystemDesign { get; set; }

        /// <example>null</example>
        public string? CommunicationFeatures { get; set; }

        /// <example>Overload, high temperature, batter low voltage.</example>
        public string? ProtectionFeatures { get; set; }

        /// <example>null</example>
        public string? Certifications { get; set; }
    }

    public record CreateAssetBatterySpecificationsRequest
    {
        /// <example>900000</example>
        public double TotalCapacity { get; set; }

        /// <example>890000</example>
        public double? UsableCapacity { get; set; }

        /// <example>Lithium-ion</example>
        public string Type { get; set; } = string.Empty;

        /// <example>220</example>
        public double Voltage { get; set; }

        /// <example>9.553</example>
        public double? DepthOfDischarge { get; set; }

        /// <example>500</example>
        public int? CycleLife { get; set; }

        /// <example>1.5</example>
        public double? ChargeDischargeRate { get; set; }

        /// <example>null</example>
        public double? Efficiency { get; set; }

        /// <example>null</example>
        public string? ManagementSystem { get; set; }

        /// <example>null</example>
        public string? WarrantyAndLifespan { get; set; }
    }
}
