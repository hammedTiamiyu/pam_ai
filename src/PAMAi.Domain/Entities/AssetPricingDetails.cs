using PAMAi.Domain.Enums;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Pricing details of an asset.
/// </summary>
public class AssetPricingDetails
{
    /// <summary>
    /// Payment plan.
    /// </summary>
    public AssetPaymentPlan PaymentPlan { get; set; }

    /// <summary>
    /// Pricing plan.
    /// </summary>
    public AssetPricingPlan PricingPlan { get; set; }

    /// <summary>
    /// Amount paid by the user.
    /// </summary>
    public decimal AmountPaid { get; set; }
}
