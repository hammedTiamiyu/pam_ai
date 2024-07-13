namespace PAMAi.Domain.Enums;

/// <summary>
/// User roles in the application.
/// </summary>
public enum ApplicationRole
{
    /// <summary>
    /// PAMAi staff and administrators.
    /// </summary>
    Administrator = 1,

    /// <summary>
    /// System installers and maintenance engineers.
    /// </summary>
    Installer = 2,

    /// <summary>
    /// Consumers of the installed renewable energy sources.
    /// </summary>
    User = 3,
}
