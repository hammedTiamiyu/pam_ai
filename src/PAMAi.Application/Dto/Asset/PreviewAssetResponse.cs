using PAMAi.Domain.Enums;

namespace PAMAi.Application.Dto.Asset;

public record PreviewAssetResponse
{
    public Guid Id { get; set; }

    /// <example>Asset name</example>
    public string Name { get; set; } = string.Empty;

    /// <example>John Doe</example>
    public string? OwnerName { get; set; } = string.Empty;

    /// <example>Plot 2322 Herbert Macaulay Way, Wuse, Abuja</example>
    public string? Location { get; set; }

    public DateTimeOffset InstallationDateUtc { get; set; }

    /// <example>1</example>
    public AssetPaymentPlan PaymentPlan { get; set; }

    /// <example>Prepaid</example>
    public string PaymentPlanValue => PaymentPlan.ToString();

    /// <example>12</example>
    public int PanelCount { get; set; }

    public static TypeAdapterConfig FromAsset => TypeAdapterConfig<Domain.Entities.Asset, PreviewAssetResponse>
        .NewConfig()
        .Map(dest => dest.PaymentPlan, src => src.PricingDetails.PaymentPlan)
        .Map(dest => dest.PanelCount, src => src.SolarSpecifications.PanelCount)
        .Map(dest => dest.OwnerName, src => src.OwnerProfile.FullName)
        .Config;
}
