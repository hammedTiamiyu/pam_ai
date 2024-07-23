namespace PAMAi.Domain.Enums;

/// <summary>
/// System design of the inverter.
/// </summary>
public enum InverterSystemDesign
{
    /// <summary>
    /// Inverter has no battery and is totally dependent on the grid.
    /// </summary>
    GridTied = 1,

    /// <summary>
    /// Inverter solely depends on energy storage in its own batteries.
    /// </summary>
    OffGrid = 2,

    /// <summary>
    /// Inverter stores energy in batteries for use and also utilises the
    /// grid in cases of power failure or low storage in the batteries.
    /// </summary>
    Hybrid = 3,
}
