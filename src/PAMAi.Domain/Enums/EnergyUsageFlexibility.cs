namespace PAMAi.Domain.Enums;

/// <summary>
/// Indicates how likely the user is able to adjust their energy usage
/// based on changes in energy rates.
/// </summary>
public enum EnergyUsageFlexibility
{
    VeryLikely = 1,
    SomewhatLikely = 2,
    Unlikely = 3,
    NotAtAll = 4,
}
