namespace PAMAi.Domain.Enums;

/// <summary>
/// Human gender.
/// </summary>
public enum Gender: byte
{
    /// <summary>
    /// Biological female.
    /// </summary>
    Female = 1,

    /// <summary>
    /// Biological male.
    /// </summary>
    Male = 2,

    // SEALED. There shall be no other gender types.
}
