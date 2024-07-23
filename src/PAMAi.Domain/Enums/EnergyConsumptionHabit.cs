namespace PAMAi.Domain.Enums;

/// <summary>
/// Energy consumption pattern of the user.
/// </summary>
public enum EnergyConsumptionHabit
{
    Consistent = 1,
    VariesSignificantly = 2,
    MostlyDuringDaylightHours = 3,
    MostlyDuringNightHours = 4,
}
