namespace PAMAi.Domain.Enums;

/// <summary>
/// Indicates the time of the day when energy is being used.
/// </summary>
[Flags]
public enum EnergyUsageTimes
{
    //None = 0,
    Morning = 1,
    Afternoon = 2,
    Evening = 4,
    Night = 8,
}
