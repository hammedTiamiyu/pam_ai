namespace PAMAi.Domain.Enums;

/// <summary>
/// Payment plan for an asset.
/// </summary>
public enum AssetPricingPlan
{
    /// <summary>
    /// Conventional pricing.
    /// </summary>
    ConventionalPricing = 1,

    /// <summary>
    /// AI dynamic pricing system.
    /// </summary>
    AIDynamicPricingSystem = 2,
}
